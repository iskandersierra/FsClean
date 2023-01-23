[<RequireQualifiedAccess>]
module Todos.Application.TodoList.CommandControllers

//open System.Threading
//open System.Threading.Tasks

//open FsClean.Domain
//open FsClean.Business.UseCases
//open FsToolkit.ErrorHandling
//open Validus

//open Todos.Domain.TodoList
//open Todos.Business.TodoList

//type TodoListId = string

//module Stateless =
//    type ExecuteCommand<'dto> = CancellationToken -> TodoListId -> 'dto -> Task<DomainResult<State.Dto>>

//    type Controller =
//        { addTask: ExecuteCommand<Command.AddTaskDto> }

//    let create (useCase: Aggregate.Stateless.UseCase<State, Event, Command>) : Controller =
//        let def = Aggregate.definition

//        let createOptions id : Aggregate.Stateless.Options<State, Event> =
//            { readState =
//                fun ct ->
//                    taskResult {
//                        let state = def.initialState ()
//                        let events = Array.empty
//                        return state, events
//                    }

//              writeState = fun ct state event -> taskResult { return () } }

//        let executeCommand ct (commandValidator: 'dto -> Result<_, ValidationErrors>) id dto =
//            taskResult {
//                let! command =
//                    commandValidator dto
//                    |> DomainError.ofValidusValidationResult

//                let! state = useCase.execute ct (createOptions id) command

//                return State.toDto state
//            }

//        { addTask = fun ct -> executeCommand ct Command.createAddTask }
