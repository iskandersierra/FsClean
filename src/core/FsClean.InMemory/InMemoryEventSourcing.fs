module FsClean.InMemory.InMemoryEventSourcing

open System
open System.Collections.Generic
open System.Globalization
open System.Threading
open System.Threading.Tasks
open FSharp.Control

open FsClean
open FsClean.Application.EventSourcing

type InMemoryEventStore<'event> =
    { append: EventStoreAppend<'event>
      getReader: EventStoreGetReader<'event>

      dump: unit -> Task<PersistedEventEnvelope<'event> []>
      clear: unit -> Task
      reset: PersistedEventEnvelope<'event> seq -> Task
      dispose: unit -> unit }

type internal InMemoryOperation<'event> =
    | AppendOp of
        CancellationToken *
        EventStoreAppendParams<'event> *
        AsyncReplyChannel<Result<EventStoreAppendResult, exn>>
    | GetReaderOp of CancellationToken * EventStoreGetReader * AsyncReplyChannel<EventStoreReader<'event>>
    | ReaderReadOp of
        CancellationToken *
        EventStoreGetReader *
        Guid *
        EventSequence *
        AsyncReplyChannel<EventSequence option * EventStoreReadResult<'event>>
    | ReaderDisposeOp of Guid * AsyncReplyChannel<unit>
    | DumpOp of AsyncReplyChannel<PersistedEventEnvelope<'event> []>
    | ResetOp of PersistedEventEnvelope<'event> [] * AsyncReplyChannel<unit>
    | DisposeOp

let create (events: PersistedEventEnvelope<'event> seq) =
    let toEventSeq (seq: uint64) = seq.ToString("x16")

    let fromEventSeq (str: string) =
        UInt64.Parse(str, NumberStyles.HexNumber)

    let inbox =
        MailboxProcessor.Start (fun inbox ->
            let events = ResizeArray events

            let calcGlobalSequence () =
                events
                |> Seq.map (fun e -> e.globalSequence)
                |> Seq.tryMax
                |> Option.map fromEventSeq
                |> Option.defaultValue 0UL

            let calcInstanceSequence pk ett eid =
                events
                |> Seq.filter (fun e ->
                    e.partitionId = pk
                    && e.entityType = ett
                    && e.entityId = eid)
                |> Seq.map (fun e -> e.instanceSequence)
                |> Seq.tryMax
                |> Option.map fromEventSeq
                |> Option.defaultValue 0UL

            let mutable globalSequence = calcGlobalSequence ()

            let readers = Dictionary()

            let rec loop () =
                async {
                    match! inbox.Receive() with
                    | AppendOp (ct, parameters, reply) ->
                        let instanceSequence =
                            calcInstanceSequence parameters.partitionId parameters.entityType parameters.entityId

                        if parameters.instanceSequence
                           <> toEventSeq instanceSequence then
                            sprintf
                                "Expected sequence %s but found %s"
                                parameters.instanceSequence
                                (toEventSeq instanceSequence)
                            |> exn
                            |> Error
                            |> reply.Reply

                        else
                            globalSequence <- globalSequence + 1UL
                            let instanceSequence = instanceSequence + 1UL

                            let persistedEvent: PersistedEventEnvelope<_> =
                                { partitionId = parameters.partitionId
                                  entityType = parameters.entityType
                                  entityId = parameters.entityId
                                  eventId = parameters.event.eventId
                                  globalSequence = toEventSeq globalSequence
                                  instanceSequence = toEventSeq instanceSequence
                                  meta = parameters.event.meta
                                  event = parameters.event.event }

                            events.Add persistedEvent

                            Ok
                                { globalSequence = toEventSeq globalSequence
                                  instanceSequence = toEventSeq instanceSequence }
                            |> reply.Reply

                    | GetReaderOp (ct, parameters, reply) ->
                        let readerId = Guid.NewGuid()
                        let mutable readerSequence = toEventSeq 0UL

                        let dispose () =
                            inbox.PostAndAsyncReply(fun reply -> ReaderDisposeOp(readerId, reply))
                            |> Async.toTask
                            :> Task

                        let reader =
                            { read =
                                fun ct ->
                                    task {
                                        let! next, result =
                                            inbox.PostAndAsyncReply (fun reply ->
                                                ReaderReadOp(ct, parameters, readerId, readerSequence, reply))

                                        match next with
                                        | Some seq -> readerSequence <- seq
                                        | None -> do! dispose ()

                                        return result
                                    }

                              dispose = dispose }

                        readers.Add(readerId, reader)

                        reply.Reply reader

                    | ReaderReadOp (ct, parameters, readerId, readerSequence, reply) ->
                        let isValidEventSubject: PersistedEventEnvelope<_> -> bool =
                            match parameters.subject with
                            | EventStoreReadAllSubjects -> fun _ -> true
                            | EventStoreReadSubjects f ->
                                match f.partitionId, f.entityType, f.entityId with
                                | Some pk, Some ett, Some eid ->
                                    fun e ->
                                        pk = e.partitionId
                                        && ett = e.entityType
                                        && eid = e.entityId
                                | None, Some ett, Some eid -> fun e -> ett = e.entityType && eid = e.entityId
                                | Some pk, None, Some eid -> fun e -> pk = e.partitionId && eid = e.entityId
                                | Some pk, Some ett, None -> fun e -> pk = e.partitionId && ett = e.entityType
                                | Some pk, None, None -> fun e -> pk = e.partitionId
                                | None, Some ett, None -> fun e -> ett = e.entityType
                                | None, None, Some eid -> fun e -> eid = e.entityId
                                | None, None, None -> fun _ -> true

                        let isValidEventFilter: PersistedEventEnvelope<_> -> bool =
                            match parameters.events with
                            | EventStoreReadAllEvents -> fun _ -> true
                            | EventStoreReadAfterGlobalSequence seq -> fun e -> e.globalSequence > seq

                        let batchSize =
                            match parameters.batchSize with
                            | None -> 25
                            | Some b -> clamp 1 100 b

                        let events =
                            events
                            |> Seq.skipWhile (fun e -> e.globalSequence <= readerSequence)
                            |> Seq.filter (fun e -> isValidEventSubject e && isValidEventFilter e)
                            |> Seq.take batchSize
                            |> Seq.toArray

                        let isLastPage = events.Length < batchSize

                        let nextSequence =
                            events
                            |> Array.tryLast
                            |> Option.map (fun e -> e.globalSequence)

                        reply.Reply(nextSequence, { events = events; isLastPage = isLastPage })

                    | ReaderDisposeOp (readerId, reply) ->
                        readers |> Dict.remove readerId |> ignore
                        reply.Reply()

                    | DumpOp reply -> reply.Reply(events.ToArray())

                    | ResetOp (data, reply) ->
                        events.Clear()
                        events.AddRange(data)
                        globalSequence <- calcGlobalSequence ()
                        reply.Reply()

                    | DisposeOp -> return ()

                    return! loop ()
                }

            loop ())

    { append =
        fun ct parameters ->
            async {
                match! inbox.PostAndAsyncReply(fun reply -> AppendOp(ct, parameters, reply)) with
                | Ok x -> return x
                | Error exn -> return raise exn
            }
            |> Async.toTask

      getReader = fun ct parameters -> task { return notImplemented () }
      
      dump =
        fun () ->
            inbox.PostAndAsyncReply(fun reply -> DumpOp(reply))
            |> Async.toTask

      reset =
          fun pairs ->
              inbox.PostAndAsyncReply(fun reply -> ResetOp(Array.ofSeq pairs, reply))
              |> Async.toTask
              :> Task

      clear =
          fun () ->
              inbox.PostAndAsyncReply(fun reply -> ResetOp(Array.empty, reply))
              |> Async.toTask
              :> Task

      dispose = fun () -> inbox.Post DisposeOp }
