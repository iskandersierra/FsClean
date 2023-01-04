module FsClean.InMemory.InMemoryKeyValueStorage

open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open FsClean
open FsClean.Application.KeyValueStorage

type InMemoryKeyValueStore<'key, 'value> =
    { save: SaveKeyValue<'key, 'value>
      remove: RemoveKeyValue<'key>
      tryLoad: TryLoadKeyValue<'key, 'value>

      dump: unit -> Task<IDictionary<'key, 'value>>
      clear: unit -> Task
      reset: ('key * 'value) seq -> Task }

type internal InMemoryOperation<'key, 'value> =
    | SaveOp of CancellationToken * 'key * 'value * AsyncReplyChannel<unit>
    | RemoveOp of CancellationToken * 'key * AsyncReplyChannel<unit>
    | TryLoadOp of CancellationToken * 'key * AsyncReplyChannel<'value option>
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

                    | DumpOp reply -> reply.Reply(Dictionary<_, _>(data))

                    | ResetOp (pairs, reply) ->
                        data.Clear()

                        for (k, v) in pairs do
                            data.Add(k, v)

                        reply.Reply()

                    return! loop ()
                }

            loop ())

    { save =
        fun ct key value ->
            mailbox.PostAndAsyncReply(fun reply -> SaveOp(ct, key, value, reply))
            |> Async.toTask
            :> Task

      remove =
          fun ct key ->
              mailbox.PostAndAsyncReply(fun reply -> RemoveOp(ct, key, reply))
              |> Async.toTask
              :> Task

      tryLoad =
          fun ct key ->
              mailbox.PostAndAsyncReply(fun reply -> TryLoadOp(ct, key, reply))
              |> Async.toTask

      dump =
          fun () ->
              mailbox.PostAndAsyncReply(fun reply -> DumpOp(reply))
              |> Async.toTask

      reset =
          fun pairs ->
              mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.ofSeq pairs, reply))
              |> Async.toTask
              :> Task

      clear =
          fun () ->
              mailbox.PostAndAsyncReply(fun reply -> ResetOp(Array.empty, reply))
              |> Async.toTask
              :> Task }

let create pairs =
    createWithComparer EqualityComparer.defaultOf<'key> pairs

let emptyWithComparer comparer = createWithComparer comparer []

let empty () = create []
