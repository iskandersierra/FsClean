namespace Todos.WebApi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open FsClean.Domain
open FsToolkit.ErrorHandling
open Validus

open Todos.Domain.TodoList
open Todos.Business.TodoList

[<CLIMutable>]
type AddTaskRequestBody =
    { title: string
      dueDate: DateTime Nullable }

[<ApiController>]
[<Route("api/rest/todo-list")>]
type TodoListController(logger: ILogger<TodoListController>) =
    inherit ControllerBase()

    let toAddTaskDto taskId (body: AddTaskRequestBody) : TodoListCommand.AddTaskDto =
        { taskId = taskId
          title = body.title
          dueDate = body.dueDate |> Option.ofNullable }

    [<HttpPost("add-task", Name = "TodoList_AddTask")>]
    member this.AddTask([<FromBody>] body: AddTaskRequestBody, ct: CancellationToken) =
        task {
            let taskId = EntityId.newGuid ()
            let commandDto = toAddTaskDto taskId body
            let command = TodoListCommand.createAddTask commandDto
            let mutable state = TodoListState.init
            let mutable events = []

            match command with
            | Error err -> return failwith "Error"
            | Ok cmd ->
                let! useCase =
                    TodoListAggregateUseCase.create
                        ct
                        { readState =
                            fun _ ->
                                Task.FromResult(Ok(state, Seq.empty))
                          writeState = fun ct state' events' ->
                            state <- state'
                            events <- events' |> Seq.toList
                            Task.FromResult(Ok()) }

                match useCase with
                | Error err ->
                    return failwith "Error creating Use Case"
                | Ok useCase ->
                    let! result = useCase.execute ct cmd
                    match result with
                    | Error err ->
                        return failwith "Error executing Use Case"
                    | Ok _ ->
                        return state
        }
