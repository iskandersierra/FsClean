namespace Todos.Domain.TodoList

open FsClean
open FsClean.Domain

module TodoListAggregate =
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
        function
        | AddTask command ->
            let taskId =
                state.tasks.Keys
                |> Seq.tryMax
                |> function
                    | Some taskId -> TaskId.value taskId + 1 |> TaskId
                    | None -> 1 |> TaskId

            let taskAdded =
                TaskAdded
                    {| taskId = taskId
                       title = command.title
                       dueDate = command.dueDate |}

            Ok [ taskAdded ]

        | RemoveTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ ->
                let taskRemoved =
                    TaskRemoved {| taskId = command.taskId |}

                Ok [ taskRemoved ]
            | None -> Ok []

        | ClearAllTasks ->
            match state.tasks |> Map.isEmpty with
            | true -> Ok []
            | false -> Ok [ AllTasksCleared ]

        | CompleteTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                let taskCompleted =
                    TaskCompleted {| taskId = command.taskId |}

                Ok [ taskCompleted ]
            | Some _ -> Ok []
            | None -> completeTaskIdDoesNotExists |> Error

        | PostponeTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                if task.dueDate = Some command.dueDate then
                    Ok []
                else
                    let taskPostponed =
                        TaskPostponed
                            {| taskId = command.taskId
                               dueDate = command.dueDate |}

                    Ok [ taskPostponed ]
            | Some _ -> Ok []
            | None -> postponeTaskIdDoesNotExists |> Error

        | KeepTaskOpen command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task ->
                if task.completed || task.dueDate = None then
                    Ok []
                else
                    let taskKeptOpen =
                        TaskKeptOpen {| taskId = command.taskId |}

                    Ok [ taskKeptOpen ]
            | None -> keepTaskOpenIdDoesNotExists |> Error
