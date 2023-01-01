module FsClean.Business.UseCases.Aggregate

open System.Threading
open System.Threading.Tasks

open FsClean.Domain
open FsToolkit.ErrorHandling

type InitialState<'state> = unit -> 'state
type ApplyEvent<'state, 'event> = 'state -> 'event -> 'state
type ExecuteCommand<'state, 'event, 'command> = 'state -> 'command -> DomainResult<'event option>

type Definition<'state, 'event, 'command> =
    { initialState: InitialState<'state>
      applyEvent: ApplyEvent<'state, 'event>
      executeCommand: ExecuteCommand<'state, 'event, 'command> }

module Stateless =

    type StateReader<'state, 'event> = CancellationToken -> Task<DomainResult<'state * 'event array>>
    type StateWriter<'state, 'event> = CancellationToken -> 'state -> 'event -> Task<DomainResult>

    type Options<'state, 'event> =
        { readState: StateReader<'state, 'event>
          writeState: StateWriter<'state, 'event> }

    type ExecuteCommand<'state, 'event, 'command> = CancellationToken -> Options<'state, 'event> -> 'command -> Task<DomainResult<'state>>

    type UseCase<'state, 'event, 'command> =
        { execute: ExecuteCommand<'state, 'event, 'command> }

    let create definition =
        { execute =
            fun ct options command ->
                taskResult {
                    let! (state, events) = options.readState ct

                    let state' =
                        events |> Seq.fold definition.applyEvent state

                    match! definition.executeCommand state command with
                    | Some event ->
                        let state'' = event |> definition.applyEvent state'
                        do! options.writeState ct state'' event
                        return state''
                    | None -> return state'
                } }
