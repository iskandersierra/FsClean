namespace Todos.WebApi.Controllers

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open FsToolkit.ErrorHandling
open Swashbuckle.AspNetCore.Annotations

open FsClean.Domain
open FsClean.Presenters.Rest

open Todos.Domain.TodoList
open Todos.Business.TodoList

module TodoListServices =
    let addTask ct (dto: TodoListCommand.AddTaskDto) =
        taskResult {
            let! command =
                TodoListCommand.createAddTask dto
                |> DomainError.ofValidusValidationResult

            let state = ref TodoListState.init
            let event = ref Unchecked.defaultof<_>

            let useCase =
                TodoListAggregateUseCase.createStateless
                    { readState = fun _ -> Task.FromResult(Ok(state.Value, Array.empty))
                      writeState =
                        fun ct state' event' ->
                            state.Value <- state'
                            event.Value <- event'
                            Task.FromResult(Ok()) }

            do! useCase.execute ct command

            let state = state.Value |> TodoListState.toDto

            return state
        }

[<CLIMutable>]
type AddTaskRequestBody =
    { title: string
      dueDate: DateTime Nullable }

[<CLIMutable>]
type TodoListTaskModel =
    { taskId: int
      title: string
      dueDate: DateTime Nullable
      completed: bool }

[<CLIMutable>]
type TodoListCommandResponseBody =
    { listId: string
      tasks: TodoListTaskModel list }

[<ApiController>]
[<Route("api/rest/todo-list")>]
type TodoListController(logger: ILogger<TodoListController>) =
    inherit ControllerBase()

    let toAddTaskDto (body: AddTaskRequestBody) : TodoListCommand.AddTaskDto =
        { title = body.title
          dueDate = body.dueDate |> Option.ofNullable }

    let toResponseBody listId (dto: TodoListState.TodoListStateDto) : TodoListCommandResponseBody =
        { listId = listId
          tasks =
            dto.tasks
            |> List.map (fun task ->
                { taskId = task.taskId
                  title = task.title
                  dueDate = task.dueDate |> Option.toNullable
                  completed = task.completed }) }

    [<HttpPost("add-task", Name = "TodoList_AddTask")>]
    [<SwaggerResponse(200, "Add task accepted", typeof<TodoListState>)>]
    member this.AddTask([<FromBody>] body: AddTaskRequestBody, ct: CancellationToken) =
        taskResult {
            let dto = toAddTaskDto body
            let listId = EntityId.newGuid ()
            let! state = TodoListServices.addTask ct dto
            let state = toResponseBody listId state
            return state
        }
        |> RestApi.toActionResult

open Google.Protobuf.FSharp.WellKnownTypes
open FsClean.Presenters.Grpc
open Todos.Grpc.TodoList

type TodoListCommandGrpcServer() =
    inherit TodoListCommandService.TodoListCommandServiceBase()

    let toAddTaskDto (body: AddTaskRequest) : TodoListCommand.AddTaskDto =
        { title = body.Title
          dueDate = Timestamp.toDateTimeOption body.DueDate }

    let toTodoListTask (dto: TodoListState.TaskStateDto) : Todos.Grpc.TodoList.TodoListTask =
        { Todos.Grpc.TodoList.TodoListTask.empty () with
            TaskId = dto.taskId
            Title = dto.title
            DueDate = Timestamp.fromDateTimeOption dto.dueDate
            Completed = dto.completed }

    let toTodoList listId (dto: TodoListState.TodoListStateDto) : Todos.Grpc.TodoList.TodoList =
        { Todos.Grpc.TodoList.TodoList.empty () with
            TodoListId = listId
            Tasks =
                dto.tasks
                |> Seq.map toTodoListTask
                |> RepeatedField.ofSeq }

    let toCommandReply (result: DomainResult<Todos.Grpc.TodoList.TodoList>) : TodoListCommandReply =
        let result =
            match result with
            | Ok result ->
                result
                |> TodoListCommandReply.Types.Result.Success
            | Error error ->
                GrpcApi.toDomainError error
                |> TodoListCommandReply.Types.Result.Error

        { TodoListCommandReply.empty () with Result = ValueOption.Some result }

    override this.AddTask request context =
        taskResult {
            let dto = toAddTaskDto request
            let listId = EntityId.newGuid ()
            let! state = TodoListServices.addTask context.CancellationToken dto
            let state = toTodoList listId state
            return state
        }
        |> Task.map toCommandReply
