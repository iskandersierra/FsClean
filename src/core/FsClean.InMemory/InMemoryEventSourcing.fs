module FsClean.InMemory.InMemoryEventSourcing

open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open FsClean
open FsClean.Application.EventSourcing
open FSharp.Control

type InMemoryEventStore =
    { dump: unit -> Task<PersistedEventEnvelope []>
      clear: unit -> Task
      reset: PersistedEventEnvelope seq -> Task }

type internal InMemoryOperation =
    | AppendOp of CancellationToken * EventStoreAppendParams * AsyncReplyChannel<EventStoreAppendResult>
    | ReadOp of CancellationToken * EventStoreReadParams * AsyncReplyChannel<AsyncSeq<EventStoreReadPage>>
    | DumpOp of AsyncReplyChannel<PersistedEventEnvelope array>
    | ResetOp of PersistedEventEnvelope [] * AsyncReplyChannel<unit>

//let create (events: PersistedEventEnvelope seq) =
//    let mailbox =
//        MailboxProcessor.Start (fun inbox ->
//            let events = ResizeArray events
//            // Subscriptions

//            let rec loop () =
//                async {
//                    match! inbox.Receive() with
//                    | AppendOp (ct, key, value, reply) ->
//                        data.[key] <- value
//                        reply.Reply()

//                    | ReadOp (ct, key, value, reply) ->
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

//let create pairs =
//    createWithComparer EqualityComparer.defaultOf<'key> pairs

//let emptyWithComparer comparer = createWithComparer comparer []

//let empty () = create []
