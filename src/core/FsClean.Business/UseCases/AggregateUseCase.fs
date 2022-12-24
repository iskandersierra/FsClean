module FsClean.Business.UseCases.AggregateUseCase

open System.Threading
open System.Threading.Tasks
open FsToolkit.ErrorHandling

open FsClean.Domain

type StateReader<'state, 'event> = CancellationToken -> Task<DomainResult<'state * 'event seq>>
type StateWriter<'state, 'event> = CancellationToken -> 'state -> 'event seq -> Task<DomainResult>

type UseCaseExecute<'command> = CancellationToken -> 'command -> Task<DomainResult>

type CommandUseCase<'command> = { execute: UseCaseExecute<'command> }

type Options<'state, 'event> =
    { readState: StateReader<'state, 'event>
      writeState: StateWriter<'state, 'event> }

type Definition<'state, 'event, 'command> =
    { applyEvent: 'state -> 'event -> 'state
      executeCommand: 'state -> 'command -> DomainResult<'event list> }

let createUseCase ct (definition: Definition<'state, 'event, 'command>) (options: Options<'state, 'event>) =
    taskResult {
        let! (initialState, initialEvents) = options.readState ct

        let mutable state =
            initialEvents
            |> Seq.fold definition.applyEvent initialState

        let execute ct command =
            taskResult {
                let! events = definition.executeCommand state command

                let state' =
                    events |> Seq.fold definition.applyEvent state

                do! options.writeState ct state' events

                state <- state'
            }

        return { execute = execute }
    }

