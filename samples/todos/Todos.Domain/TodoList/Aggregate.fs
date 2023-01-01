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

    let execute state =
        let ok = Some >> Ok
        let none = Ok None

        function
        | AddTask command ->
            let taskId =
                state.tasks.Keys
                |> Seq.tryMax
                |> function
                    | Some taskId -> TaskId.value taskId + 1 |> TaskId
                    | None -> 1 |> TaskId

            TaskAdded
                {| taskId = taskId
                   title = command.title
                   dueDate = command.dueDate |}
            |> ok

        | RemoveTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ -> TaskRemoved {| taskId = command.taskId |} |> ok
            | None -> none

        | ClearAllTasks ->
            match state.tasks |> Map.isEmpty with
            | true -> none
            | false -> AllTasksCleared |> ok

        | CompleteTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed -> TaskCompleted {| taskId = command.taskId |} |> ok
            | Some _ -> none
            | None -> completeTaskIdDoesNotExists |> Error

        | PostponeTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                if task.dueDate = Some command.dueDate then
                    none
                else
                    TaskPostponed
                        {| taskId = command.taskId
                           dueDate = command.dueDate |}
                    |> ok
            | Some _ -> none
            | None -> postponeTaskIdDoesNotExists |> Error

        | KeepTaskOpen command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task ->
                if task.completed || task.dueDate = None then
                    none
                else
                    TaskKeptOpen {| taskId = command.taskId |} |> ok
            | None -> keepTaskOpenIdDoesNotExists |> Error
