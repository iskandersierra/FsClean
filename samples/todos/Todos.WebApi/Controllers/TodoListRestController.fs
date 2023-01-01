namespace Todos.WebApi.Controllers

open System
open System.Threading
open Microsoft.AspNetCore.Mvc

open FsClean.Application
open FsClean.Presenters.Rest
open FsToolkit.ErrorHandling
open Swashbuckle.AspNetCore.Annotations

open Todos.Domain.TodoList
open Todos.Application.TodoList

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
type TodoListModel =
    { listId: string
      tasks: TodoListTaskModel seq }

[<CLIMutable>]
type TodoListCommandResponseBody = { todoList: TodoListModel }

[<ApiController>]
[<Route("api/rest/todo-list")>]
type TodoListRestController(commands: CommandControllers.Stateless.Controller, idGen: IdGenerator) =
    inherit ControllerBase()

    let toAddTaskDto (body: AddTaskRequestBody) : Command.AddTaskDto =
        { title = body.title
          dueDate = body.dueDate |> Option.ofNullable }

    let toTaskModel (dto: State.TaskDto) =
        { TodoListTaskModel.taskId = dto.taskId
          title = dto.title
          dueDate = dto.dueDate |> Option.toNullable
          completed = dto.completed }

    let toResponseBody listId (dto: State.Dto) : TodoListCommandResponseBody =
        { todoList =
            { listId = listId
              tasks = dto.tasks |> Seq.map toTaskModel |> Seq.toArray } }

    let handleCommand ct listId toCommandDto body =
        taskResult {
            let dto = toCommandDto body
            let! stateDto = commands.addTask ct listId dto
            let state = toResponseBody listId stateDto
            return state
        }
        |> RestApi.ActionResult.toOk

    [<HttpPost("add-task", Name = "TodoList_AddTask")>]
    [<SwaggerResponse(200, "Add task completed", typeof<State>)>]
    [<SwaggerResponse(400, "Validation errors", typeof<ValidationProblemDetails>)>]
    [<SwaggerResponse(401, "Unauthorized", typeof<ProblemDetails>)>]
    [<SwaggerResponse(404, "Entity not found", typeof<ProblemDetails>)>]
    [<SwaggerResponse(409, "Conflict errors", typeof<ValidationProblemDetails>)>]
    [<SwaggerResponse(500, "Service failure", typeof<ProblemDetails>)>]
    member this.AddTask([<FromBody>] body: AddTaskRequestBody, ct: CancellationToken) =
        handleCommand ct (idGen.generate ()) toAddTaskDto body
