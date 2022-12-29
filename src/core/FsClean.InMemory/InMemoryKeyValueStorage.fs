module FsClean.InMemory.InMemoryKeyValueStorage

open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open FsClean
open FsClean.Application.KeyValueStorage

type InMemoryKeyValueStore<'key, 'value> =
    { dump: unit -> Task<IDictionary<'key, 'value>>
      clear: unit -> Task
      reset: ('key * 'value) seq -> Task }

type internal InMemoryOperation<'key, 'value> =
    | SaveOp of CancellationToken * 'key * 'value * AsyncReplyChannel<unit>
    | RemoveOp of CancellationToken * 'key * AsyncReplyChannel<unit>
    | TryLoadOp of CancellationToken * 'key * AsyncReplyChannel<'value option>
    | TryLoadManyOp of CancellationToken * 'key array * AsyncReplyChannel<('key * 'value) array>
    | TryLoadFirstOp of CancellationToken * 'key array * AsyncReplyChannel<('key * 'value) option>
    | DumpOp of AsyncReplyChannel<IDictionary<'key, 'value>>
    | ResetOp of ('key * 'value) array * AsyncReplyChannel<unit>

let createWithComparer comparer pairs =

    let mailbox =
        MailboxProcessor.Start (fun inbox ->
            let pairs = Seq.map KeyValuePair.Create pairs
            let data = Dictionary<_, _>(pairs, comparer)

            let rec loop () =
                async {
                    match! inbox.Receive() with
                    | SaveOp (ct, key, value, reply) ->
                        data.[key] <- value
                        reply.Reply()

                    | RemoveOp (ct, key, reply) ->
                        data.Remove key |> ignore
                        reply.Reply()

                    | TryLoadOp (ct, key, reply) ->
                        match data.TryGetValue key with
                        | true, value -> reply.Reply(Some value)
                        | false, _ -> reply.Reply None

                    | TryLoadManyOp (ct, keys, reply) ->
                        let values =
                            keys
                            |> Seq.collect (fun key ->
                                match data.TryGetValue key with
                                | true, value -> [ key, value ]
                                | false, _ -> [])
                            |> Seq.toArray

                        reply.Reply values

                    | TryLoadFirstOp (ct, keys, reply) ->
                        let value =
                            keys
                            |> Seq.collect (fun key ->
                                match data.TryGetValue key with
                                | true, value -> [ key, value ]
                                | false, _ -> [])
                            |> Seq.tryHead

                        reply.Reply value

                    | DumpOp reply -> reply.Reply(Dictionary<_, _>(data))

                    | ResetOp (pairs, reply) ->
                        data.Clear()

                        for (k, v) in pairs do
                            data.Add(k, v)

                        reply.Reply()

                    return! loop ()
                }

            loop ())

    let save ct key value =
        mailbox.PostAndAsyncReply(fun reply -> SaveOp(ct, key, value, reply))
        |> Async.toTask
        :> Task

    let remove ct key =
        mailbox.PostAndAsyncReply(fun reply -> RemoveOp(ct, key, reply))
        |> Async.toTask
        :> Task

    let tryLoad ct key =
        mailbox.PostAndAsyncReply(fun reply -> TryLoadOp(ct, key, reply))
        |> Async.toTask

    let tryLoadMany ct keys =
        mailbox.PostAndAsyncReply(fun reply -> TryLoadManyOp(ct, Seq.toArray keys, reply))
        |> Async.toTask

    let tryLoadFirst ct keys =
        mailbox.PostAndAsyncReply(fun reply -> TryLoadFirstOp(ct, Seq.toArray keys, reply))
        |> Async.toTask

    let dump () =
        mailbox.PostAndAsyncReply(fun reply -> DumpOp(reply))
        |> Async.toTask

    let reset pairs =
        mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.ofSeq pairs, reply))
        |> Async.toTask
        :> Task

    let clear () =
        mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.empty, reply))
        |> Async.toTask
        :> Task

    { save = save
      remove = remove
      tryLoad = tryLoad
      tryLoadMany = tryLoadMany
      tryLoadFirst = tryLoadFirst },
    { dump = dump
      reset = reset
      clear = clear }

let create pairs =
    createWithComparer EqualityComparer.defaultOf<'key> pairs

let emptyWithComparer comparer = createWithComparer comparer []

let empty () = create []
