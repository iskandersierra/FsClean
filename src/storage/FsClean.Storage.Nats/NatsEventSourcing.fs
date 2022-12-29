module FsClean.Storage.Nats.NatsEventSourcing

open NATS.Client
open NATS.Client.JetStream
open NATS.Client.KeyValue

open FsClean
open FsClean.Application.EventSourcing

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
            .Build()

    management.AddStream(config)

let initialize =
    StreamConfiguration.Builder().Build()
    |> initializeWithConfig

type PayloadType = byte []

let create options : EventStore<PayloadType> =
    let stream =
        options.conn.CreateJetStreamContext(options.jetStreamOptions)

    let subjectFor = sprintf "%s.PK.%s.ETT.%s.ID.%s"

    let headerFor pk ett id (seq: uint64) meta =
        let header = MsgHeader()
        header.Add("pk", pk)
        header.Add("ett", ett)
        header.Add("id", id)
        header.Add("seq", seq.ToString("x16"))

        meta
        |> Map.iter (fun k v -> header.Add($"meta-{k}", v))

        header

    { append =
        fun ct parameters ->
            task {
                // Validations
                let instanceSequence = parameters.entitySequence + 1UL

                let subject =
                    subjectFor options.subjectPrefix parameters.partitionId parameters.entityType parameters.entityId

                let header =
                    headerFor
                        parameters.partitionId
                        parameters.entityType
                        parameters.entityId
                        instanceSequence
                        parameters.event.meta

                let data = parameters.event.event

                let msg = Msg(subject, header, data)

                let pubOptions =
                    PublishOptions
                        .Builder()
                        .WithStream(options.streamName)
                        .WithMessageId(parameters.event.eventId)
                        .WithExpectedLastSubjectSequence(parameters.entitySequence)
                        .Build()

                let! ack = stream.PublishAsync(msg, pubOptions)

                return
                    { globalSequence = ack.Seq
                      instanceSequence = instanceSequence }
            }

      read = fun ct parameters -> task { return notImplemented() } }
