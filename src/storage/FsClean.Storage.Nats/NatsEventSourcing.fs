module FsClean.Storage.Nats.NatsEventSourcing

open System
open System.Globalization
open System.Threading.Tasks

open FsClean
open FsClean.Application.EventSourcing
open NATS.Client
open NATS.Client.JetStream

type PayloadType = byte []

type NatsJetStreamEventStore =
    { append: EventStoreAppend<PayloadType>
      getReader: EventStoreGetReader<PayloadType> }

type Options =
    { conn: IConnection
      streamName: string
      subjectPrefix: string
      jetStreamOptions: JetStreamOptions }

let defaultOptions =
    { conn = null
      streamName = "ESEvents"
      subjectPrefix = "ESEvents"
      jetStreamOptions = JetStreamOptions.DefaultJsOptions }

let initializeWithConfig config options =
    let management =
        options.conn.CreateJetStreamManagementContext(options.jetStreamOptions)

    let config =
        StreamConfiguration
            .Builder(config)
            .WithName(options.streamName)
            .WithSubjects([| sprintf "%s.>" options.subjectPrefix |])
            .Build()

    management.AddStream(config)

let initialize =
    StreamConfiguration.Builder().Build()
    |> initializeWithConfig

[<Literal>]
let DefaultBatchSize = 25

[<Literal>]
let MinBatchSize = 1

[<Literal>]
let MaxBatchSize = 100

[<Literal>]
let FetchTimeoutMilliseconds = 1000

let create options : NatsJetStreamEventStore =
    let subjectFor = sprintf "%s.PK.%s.ETT.%s.ID.%s"

    let toEventSeq (seq: uint64) = seq.ToString("x16")

    let fromEventSeq (str: string) =
        UInt64.Parse(str, NumberStyles.HexNumber)

    let headerFor pk ett eid mid (seq: uint64) meta =
        let header = MsgHeader()
        header.Add("pk", pk)
        header.Add("ett", ett)
        header.Add("eid", eid)
        header.Add("mid", mid)
        header.Add("seq", toEventSeq seq)

        meta
        |> Map.iter (fun k v -> header.Add($"meta-{k}", v))

        header

    let toMessage pk ett eid mid seq meta data =
        let subject =
            subjectFor options.subjectPrefix pk ett eid

        let header = headerFor pk ett eid mid seq meta

        Msg(subject, header, data)

    let fromMessage (msg: Msg) : PersistedEventEnvelope<byte []> =
        let getHeader name =
            if msg.HasHeaders then
                msg.Header.[name]
            else
                null

        let meta =
            msg.Header.Keys
            |> Seq.cast<string>
            |> Seq.filter (fun k -> k.StartsWith("meta-"))
            |> Seq.map (fun k -> k.Substring(5), msg.Header.[k])
            |> Map.ofSeq

        { partitionId = getHeader "pk"
          entityType = getHeader "ett"
          entityId = getHeader "eid"
          eventId = getHeader "mid"
          globalSequence = toEventSeq msg.MetaData.StreamSequence
          instanceSequence = getHeader "seq"
          meta = meta
          event = msg.Data }

    let stream =
        options.conn.CreateJetStreamContext(options.jetStreamOptions)

    { append =
        fun ct parameters ->
            task {
                // Validations
                let instanceSequenceInt =
                    parameters.instanceSequence
                    |> fromEventSeq
                    |> (+) 1UL

                let msg =
                    toMessage
                        parameters.partitionId
                        parameters.entityType
                        parameters.entityId
                        parameters.event.eventId
                        instanceSequenceInt
                        parameters.event.meta
                        parameters.event.event

                let pubOptions =
                    PublishOptions
                        .Builder()
                        .WithStream(options.streamName)
                        .WithMessageId(parameters.event.eventId)
                        .WithExpectedLastSubjectSequence(instanceSequenceInt - 1UL)
                        .Build()

                let! ack = stream.PublishAsync(msg, pubOptions)

                return
                    { globalSequence = toEventSeq ack.Seq
                      instanceSequence = toEventSeq instanceSequenceInt }
            }

      getReader =
          fun ct parameters ->
              task {
                  let filterSubject =
                      match parameters.subject with
                      | EventStoreReadAllSubjects -> sprintf "%s.>" options.subjectPrefix
                      | EventStoreReadSubjects filter ->
                          options.subjectPrefix
                          |> fun subject ->
                              match filter.partitionId with
                              | Some pk -> $"{subject}.PK.{pk}"
                              | None -> $"{subject}.PK.*"
                          |> fun subject ->
                              match filter.entityType with
                              | Some pk -> $"{subject}.ETT.{pk}"
                              | None -> $"{subject}.ETT.*"
                          |> fun subject ->
                              match filter.entityId with
                              | Some pk -> $"{subject}.ID.{pk}"
                              | None -> $"{subject}.ID.*"

                  let deliverPolicy (builder: ConsumerConfiguration.ConsumerConfigurationBuilder) =
                      match parameters.events with
                      | EventStoreReadAllEvents ->
                        builder.WithDeliverPolicy(DeliverPolicy.All)
                      | EventStoreReadAfterGlobalSequence seq ->
                        builder
                            .WithDeliverPolicy(DeliverPolicy.ByStartSequence)
                            .WithStartSequence(fromEventSeq seq)

                  let batchSize =
                      match parameters.batchSize with
                      | None -> DefaultBatchSize
                      | Some b -> clamp MinBatchSize MaxBatchSize b

                  let consumerConfig =
                      let builder =
                          ConsumerConfiguration
                              .Builder()
                              .WithAckPolicy(AckPolicy.None)
                              .WithMemStorage(true)
                              .WithFilterSubject(filterSubject)
                          |> deliverPolicy

                      builder.Build()

                  let subOptions =
                      PullSubscribeOptions
                          .Builder()
                          .WithStream(options.streamName)
                          .WithConfiguration(consumerConfig)
                          .Build()

                  let sub =
                      stream.PullSubscribe(filterSubject, subOptions)

                  return
                      { read =
                          fun ct ->
                              task {
                                  let result =
                                      sub.Fetch(batchSize, FetchTimeoutMilliseconds)

                                  return
                                      { isLastPage = result.Count < batchSize
                                        events =
                                          result
                                          |> Seq.map (fun msg -> fromMessage msg)
                                          |> Seq.toArray }
                              }

                        dispose =
                            fun () ->
                                task {
                                    do! sub.DrainAsync()
                                    sub.Dispose()
                                } }
              } }
