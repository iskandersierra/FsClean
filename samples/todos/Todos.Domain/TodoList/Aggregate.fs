namespace Todos.Domain.TodoList

open FsClean

module Aggregate =
    let execute state =
        function
        | AddTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ ->
                Errors.TaskIdAlreadyExistsConflict
                |> DomainError.setSomeOperation Operations.ADD_TASK
                |> Error
            | None ->
                let task: TaskAdded =
                    { taskId = command.taskId
                      title = command.title
                      dueDate = command.dueDate }

                Ok [ TaskAdded task ]

        | RemoveTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ ->
                let task: TaskRemoved = { taskId = command.taskId }
                Ok [ TaskRemoved task ]

            | None -> Ok []

        | ClearAllTasks ->
            match state.tasks |> Map.isEmpty with
            | true -> Ok []
            | false -> Ok [ AllTasksCleared ]

        | CompleteTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                let task: TaskCompleted = { taskId = command.taskId }
                Ok [ TaskCompleted task ]

            | Some _ -> Ok []

            | None ->
                Errors.TaskIdDoesNotExistsConflict
                |> DomainError.setSomeOperation Operations.COMPLETE_TASK
                |> Error

        | PostponeTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                let task: TaskPostponed =
                    { taskId = command.taskId
                      dueDate = command.dueDate }

                Ok [ TaskPostponed task ]

            | Some _ -> Ok []

            | None ->
                Errors.TaskIdDoesNotExistsConflict
                |> DomainError.setSomeOperation Operations.POSTPONE_TASK
                |> Error

        | KeepTaskOpen command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                match task.dueDate with
                | Some _ ->
                    let task: TaskKeptOpen = { taskId = command.taskId }
                    Ok [ TaskKeptOpen task ]
                | None -> Ok []

            | Some _ -> Ok []

            | None ->
                Errors.TaskIdDoesNotExistsConflict
                |> DomainError.setSomeOperation Operations.KEEP_TASK_OPEN
                |> Error
