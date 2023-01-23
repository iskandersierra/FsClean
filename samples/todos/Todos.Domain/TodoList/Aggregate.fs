namespace Todos.Domain.TodoList

open FsClean
open FsClean.Domain

module Aggregate =
    let completeTaskIdDoesNotExists =
        Errors.TaskIdDoesNotExistsConflict
        |> DomainError.setOperation Operations.COMPLETE_TASK

    let postponeTaskIdDoesNotExists =
        Errors.TaskIdDoesNotExistsConflict
        |> DomainError.setOperation Operations.POSTPONE_TASK

    let keepTaskOpenIdDoesNotExists =
        Errors.TaskIdDoesNotExistsConflict
        |> DomainError.setOperation Operations.KEEP_TASK_OPEN

    let internal taskIdOne = TaskId.ofValue 1
    let internal nothing: DomainResult<Event list> = Ok []

    let execute state command =
        match command with
        | AddTask command ->
            let taskId =
                state.tasks.Keys
                |> Seq.tryMax
                |> function
                    | Some taskId -> taskId + taskIdOne
                    | None -> taskIdOne

            Ok [ TaskAdded {| taskId = taskId; title = command.title; dueDate = command.dueDate |} ]

        | RemoveTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ -> Ok [ TaskRemoved {| taskId = command.taskId |} ]
            | None -> nothing

        | ClearAllTasks ->
            match state.tasks |> Map.isEmpty with
            | true -> nothing
            | false -> Ok [ AllTasksCleared ]

        | CompleteTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed -> Ok [ TaskCompleted {| taskId = command.taskId |} ]
            | Some _ -> nothing
            | None -> Error completeTaskIdDoesNotExists

        | PostponeTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                if task.dueDate = Some command.dueDate then
                    nothing
                else
                    Ok [ TaskPostponed
                             {| taskId = command.taskId
                                dueDate = command.dueDate |} ]
            | Some _ -> nothing
            | None -> Error postponeTaskIdDoesNotExists

        | KeepTaskOpen command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task ->
                if task.completed || task.dueDate = None then
                    nothing
                else
                    Ok [ TaskKeptOpen {| taskId = command.taskId |} ]
            | None -> Error keepTaskOpenIdDoesNotExists
