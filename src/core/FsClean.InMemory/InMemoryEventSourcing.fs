module FsClean.InMemory.InMemoryEventSourcing

open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open FsClean
open FsClean.Application.EventSourcing
open FSharp.Control

type InMemoryEventStore<'event> =
    { dump: unit -> Task<PersistedEventEnvelope<'event> []>
      clear: unit -> Task
      reset: PersistedEventEnvelope<'event> seq -> Task }

type internal InMemoryOperation<'event> =
    | AppendOp of CancellationToken * EventStoreAppendParams<'event> * AsyncReplyChannel<EventStoreAppendResult>
    | ReadOp of CancellationToken * EventStoreReadParams * AsyncReplyChannel<EventStoreReadResult<'event>>
    | DumpOp of AsyncReplyChannel<PersistedEventEnvelope<'event> []>
    | ResetOp of PersistedEventEnvelope<'event> [] * AsyncReplyChannel<unit>

//let create (events: PersistedEventEnvelope<'event> seq) : EventStore<'event> =
//    let mailbox =
//        MailboxProcessor.Start (fun inbox ->
//            let events = ResizeArray events
//            // Subscriptions

//            let rec loop () =
//                async {
//                    match! inbox.Receive() with
//                    | AppendOp (ct, parameters, reply) ->
//                        let mutable currentVersion = parameters.currentVersion

//                        parameters.events
//                        |> Seq.mapi (fun i e ->
//                            currentVersion <- parameters.currentVersion + uint64 i + 1UL

//                            { partitionId = parameters.partitionId
//                              entityType = parameters.entityType
//                              entityId = parameters.entityId
//                              entitySequence = currentVersion
//                              meta = e.meta
//                              eventId = e.eventId
//                              event = e.event })
//                        |> events.AddRange

//                        reply.Reply { currentVersion = currentVersion }

//                    | ReadOp (ct, parameters, reply) ->
//                        data.[key] <- value
//                        reply.Reply()

//                    | DumpOp reply -> reply.Reply(Dictionary<_, _>(data))

//                    | ResetOp (pairs, reply) ->
//                        data.Clear()

//                        for (k, v) in pairs do
//                            data.Add(k, v)

//                        reply.Reply()

//                    return! loop ()
//                }

//            loop ())

//    let save ct key value =
//        mailbox.PostAndAsyncReply(fun reply -> SaveOp(ct, key, value, reply))
//        |> Async.toTask
//        :> Task

//    let remove ct key =
//        mailbox.PostAndAsyncReply(fun reply -> RemoveOp(ct, key, reply))
//        |> Async.toTask
//        :> Task

//    let tryLoad ct key =
//        mailbox.PostAndAsyncReply(fun reply -> TryLoadOp(ct, key, reply))
//        |> Async.toTask

//    let tryLoadMany ct keys =
//        mailbox.PostAndAsyncReply(fun reply -> TryLoadManyOp(ct, Seq.toArray keys, reply))
//        |> Async.map Array.toSeq
//        |> Async.toTask

//    let tryLoadFirst ct keys =
//        mailbox.PostAndAsyncReply(fun reply -> TryLoadFirstOp(ct, Seq.toArray keys, reply))
//        |> Async.toTask

//    let dump () =
//        mailbox.PostAndAsyncReply(fun reply -> DumpOp(reply))
//        |> Async.toTask

//    let reset pairs =
//        mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.ofSeq pairs, reply))
//        |> Async.toTask
//        :> Task

//    let clear () =
//        mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.empty, reply))
//        |> Async.toTask
//        :> Task

//    { save = save
//      remove = remove
//      tryLoad = tryLoad
//      tryLoadMany = tryLoadMany
//      tryLoadFirst = tryLoadFirst },
//    { dump = dump
//      reset = reset
//      clear = clear }

//let empty () = create []
