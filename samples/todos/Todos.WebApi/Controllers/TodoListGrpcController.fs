namespace Todos.WebApi.Controllers

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open FsClean.Domain
open FsClean.Application
open FsClean.Presenters.Grpc
open FsToolkit.ErrorHandling
open Google.Protobuf.FSharp.WellKnownTypes
open Swashbuckle.AspNetCore.Annotations

open Todos.Domain.TodoList
open Todos.Business.TodoList
open Todos.Application.TodoList
open Todos.Grpc.TodoList

//module TodoListServices =
//    let addTask ct (dto: TodoListCommand.AddTaskDto) =
//        taskResult {
//            let! command =
//                TodoListCommand.createAddTask dto
//                |> DomainError.ofValidusValidationResult

//            let state = ref TodoListState.init
//            let event = ref Unchecked.defaultof<_>

//            let useCase =
//                TodoListAggregateUseCase.createStateless
//                    { readState = fun _ -> Task.FromResult(Ok(state.Value, Array.empty))
//                      writeState =
//                        fun ct state' event' ->
//                            state.Value <- state'
//                            event.Value <- event'
//                            Task.FromResult(Ok()) }

//            do! useCase.execute ct command

//            let state = state.Value |> TodoListState.toDto

//            return state
//        }


//type TodoListGrpcServer() =
//    inherit TodoListCommandService.TodoListCommandServiceBase()

//    let toAddTaskDto (body: AddTaskRequest) : TodoListCommand.AddTaskDto =
//        { title = body.Title
//          dueDate = Timestamp.toDateTimeOption body.DueDate }

//    let toTodoListTask (dto: TodoListState.TaskDto) : Todos.Grpc.TodoList.TodoListTask =
//        { Todos.Grpc.TodoList.TodoListTask.empty () with
//            TaskId = dto.taskId
//            Title = dto.title
//            DueDate = Timestamp.fromDateTimeOption dto.dueDate
//            Completed = dto.completed }

//    let toTodoList listId (dto: TodoListState.TodoListStateDto) : Todos.Grpc.TodoList.TodoList =
//        { Todos.Grpc.TodoList.TodoList.empty () with
//            TodoListId = listId
//            Tasks =
//                dto.tasks
//                |> Seq.map toTodoListTask
//                |> RepeatedField.ofSeq }

//    let toCommandReply (result: DomainResult<Todos.Grpc.TodoList.TodoList>) : TodoListCommandReply =
//        let result =
//            match result with
//            | Ok result ->
//                result
//                |> TodoListCommandReply.Types.Result.Success
//            | Error error ->
//                GrpcApi.toDomainError error
//                |> TodoListCommandReply.Types.Result.Error

//        { TodoListCommandReply.empty () with Result = ValueOption.Some result }

//    override this.AddTask request context =
//        taskResult {
//            let dto = toAddTaskDto request
//            let listId = EntityId.newGuid ()
//            let! state = TodoListServices.addTask context.CancellationToken dto
//            let state = toTodoList listId state
//            return state
//        }
//        |> Task.map toCommandReply
