namespace FsClean

/// A module that provides atomic variable creation.
/// The atomic variable is a mutable variable that can be updated atomically.
/// Implementation is based on MailboxProcessor.
/// Functions directly declared in this module receive an async update function
/// that returns a new value.
/// <seealso cref="WithResult" />
[<RequireQualifiedAccess>]
module AtomicVar =
    let inline internal makeUpdateFn (inbox: MailboxProcessor<_>) =
        fun updateFn ->
            async {
                match! inbox.PostAndAsyncReply(fun reply -> Choice1Of2(updateFn, reply)) with
                | Ok result -> return result
                | Error exn -> return Exn.rethrow exn
            }

    let inline internal makeDisposeFn (inbox: MailboxProcessor<_>) = fun () -> inbox.Post(Choice2Of2())

    /// A module that provides atomic variable creation with results on each update.
    /// The atomic variable is a mutable variable that can be updated atomically.
    /// Implementation is based on MailboxProcessor.
    /// Functions directly declared in this module receive an async update function
    /// that returns a new value and a result.
    module WithResult =
        /// AtomicVar represents a mutable variable that can be updated atomically.
        /// The update function returns a result
        type AtomicVar<'value, 'result> =
            { update: ('value -> Async<'value * 'result>) -> Async<'result>
              dispose: unit -> unit }

        /// Creates an atomic variable with the given initial value.
        /// The update function returns a result.
        let create (initialValue: 'value) =
            let inbox =
                MailboxProcessor.Start (fun inbox ->
                    let rec loop value =
                        async {
                            match! inbox.Receive() with
                            | Choice1Of2 (((updateFn: 'value -> ('value * 'result) Async),
                                           (reply: AsyncReplyChannel<Result<'result, exn>>))) ->
                                try
                                    let! value', result = updateFn value
                                    reply.Reply(Ok result)
                                    return! loop value'
                                with
                                | exn ->
                                    reply.Reply(Error exn)
                                    return! loop value

                            | Choice2Of2 () -> return ()
                        }

                    loop initialValue)

            { update = makeUpdateFn inbox
              dispose = makeDisposeFn inbox }

    /// AtomicVar represents a mutable variable that can be updated atomically.
    /// The update function does not return a result.
    type AtomicVar<'value> =
        { update: ('value -> Async<'value>) -> Async<unit>
          dispose: unit -> unit }

    /// Creates an atomic variable with the given initial value.
    /// The update function does not return a result.
    let create (initialValue: 'value) =
        let inbox =
            MailboxProcessor.Start (fun inbox ->
                let rec loop value =
                    async {
                        match! inbox.Receive() with
                        | Choice1Of2 (((updateFn: 'value -> 'value Async), (reply: AsyncReplyChannel<Result<unit, exn>>))) ->
                            try
                                let! value' = updateFn value
                                reply.Reply(Ok())
                                return! loop value'
                            with
                            | exn ->
                                reply.Reply(Error exn)
                                return! loop value

                        | Choice2Of2 () -> return ()
                    }

                loop initialValue)

        { update = makeUpdateFn inbox
          dispose = makeDisposeFn inbox }
