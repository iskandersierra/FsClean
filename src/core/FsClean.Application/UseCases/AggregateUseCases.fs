namespace FsClean.Application.UseCases

open System

open FsClean
open FsClean.Domain
open FsClean.Application.KeyValueStorage
open FsToolkit.ErrorHandling

type IAggregateDefinition<'state, 'event, 'command> =
    abstract init : unit -> 'state
    abstract apply : 'state -> 'event -> 'state
    abstract execute : 'state -> 'command -> DomainResult<'event seq>

module AggregateDefinition =
    let forDomain init apply execute =
        { new IAggregateDefinition<'state, 'event, 'command> with
            member _.init() = init ()
            member _.apply state event = apply state event
            member _.execute state command = execute state command }

module AggregateUseCase =
    module OnKVStorage =
        module Stateless =
            let internal updateStore
                (definition: IAggregateDefinition<'state, 'event, 'command>)
                (kvstore: IKVStore<'key, 'state>)
                key
                command
                stateOpt
                : AsyncDomainResult<'state> =
                asyncResult {
                    let state =
                        match stateOpt with
                        | ValueSome state -> state
                        | ValueNone -> definition.init ()

                    let! events = definition.execute state command

                    let state' =
                        events |> Seq.fold definition.apply state

                    do! kvstore.set key state'

                    return state'
                }

            /// Creates a use case that updates the aggregate state persisted in the given key-value store.
            /// The use case is stateless, meaning that it does not cache the aggregate state in memory
            /// and always reads the state from the key-value store before executing the command.
            let create (definition: IAggregateDefinition<'state, 'event, 'command>) (kvstore: IKVStore<'key, 'state>) =
                { new IUseCase<struct ('key * 'command), unit> with
                    member this.Execute((key, command)) =
                        asyncResult {
                            let! stateOpt = kvstore.tryGet key
                            let! _state = updateStore definition kvstore key command stateOpt
                            return ()
                        } }

        module Stateful =
            /// Creates a use case that updates the aggregate state persisted in the given key-value store.
            /// The use case is stateful, meaning that it caches the aggregate state in memory and only
            /// reads the state from the key-value store when the cache is empty.
            /// It is assumed that the client manages thread-safety of the use case.
            let createSync
                (definition: IAggregateDefinition<'state, 'event, 'command>)
                (kvstore: IKVStore<'key, 'state>)
                =
                let mutable cache = ValueNone

                { new IUseCase<struct ('key * 'command), unit> with
                    member this.Execute((key, command)) =
                        asyncResult {
                            let! stateOpt =
                                match cache with
                                | ValueNone -> kvstore.tryGet key
                                | ValueSome _ -> async.Return cache

                            let! state = Stateless.updateStore definition kvstore key command stateOpt
                            cache <- ValueSome state
                        } }

            /// Creates a use case that updates the aggregate state persisted in the given key-value store.
            /// The use case is stateful, meaning that it caches the aggregate state in memory and only
            /// reads the state from the key-value store when the cache is empty.
            /// It is not assumed that the client manages thread-safety of the use case, so the use case
            /// uses an atomic variable to ensure thread-safety.
            let create (definition: IAggregateDefinition<'state, 'event, 'command>) (kvstore: IKVStore<'key, 'state>) =
                let atomicCache =
                    AtomicVar.WithResult.create (ValueNone: 'state voption)

                { new IUseCase<struct ('key * 'command), unit> with
                    member this.Execute((key, command)) =
                        atomicCache.update (fun stateOpt ->
                            async {
                                let! stateOpt =
                                    match stateOpt with
                                    | ValueNone -> kvstore.tryGet key
                                    | ValueSome _ -> async.Return stateOpt

                                match! Stateless.updateStore definition kvstore key command stateOpt with
                                | Ok state -> return ValueSome state, Ok()
                                | Error e -> return stateOpt, Error e
                            })
                  interface IDisposable with
                      member this.Dispose() = atomicCache.dispose () }
