module FsClean.Business.UseCases.AggregateUseCase

open System.Threading
open System.Threading.Tasks
open FsToolkit.ErrorHandling

open FsClean.Domain

type StateReader<'state, 'event> = CancellationToken -> Task<DomainResult<'state * 'event array>>
type StateWriter<'state, 'event> = CancellationToken -> 'state -> 'event -> Task<DomainResult>

type Options<'state, 'event> =
    { readState: StateReader<'state, 'event>
      writeState: StateWriter<'state, 'event> }

type ApplyEvent<'state, 'event> = 'state -> 'event -> 'state
type ExecuteCommand<'state, 'event, 'command> = 'state -> 'command -> DomainResult<'event option>


type Definition<'state, 'event, 'command> =
    { applyEvent: ApplyEvent<'state, 'event>
      executeCommand: ExecuteCommand<'state, 'event, 'command> }

type UseCaseExecute<'command> = CancellationToken -> 'command -> Task<DomainResult>

type CommandUseCase<'command> = { execute: UseCaseExecute<'command> }

let create ct definition options =
    taskResult {
        let! (initialState, initialEvents) = options.readState ct

        let mutable state =
            initialEvents
            |> Seq.fold definition.applyEvent initialState

        let execute ct command =
            taskResult {
                match! definition.executeCommand state command with
                | Some event ->
                    let state' = event |> definition.applyEvent state
                    do! options.writeState ct state' event
                    state <- state'
                | None -> ()
            }

        return { execute = execute }
    }

let createStateless definition options =
    let execute ct command =
        taskResult {
            let! (initialState, initialEvents) = options.readState ct

            let state =
                initialEvents
                |> Seq.fold definition.applyEvent initialState

            match! definition.executeCommand state command with
            | Some event ->
                let state' = event |> definition.applyEvent state
                do! options.writeState ct state' event
            | None -> ()
        }

    { execute = execute }
