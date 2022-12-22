﻿namespace Todos.Domain.TodoList

open FsClean.Domain

module TodoListAggregate =
    let addTaskIdAlreadyExists =
        Errors.TaskIdAlreadyExistsConflict
        |> DomainError.setOperation Operations.ADD_TASK

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
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ -> addTaskIdAlreadyExists |> Error
            | None ->
                let task =
                    {| taskId = command.taskId
                       title = command.title
                       dueDate = command.dueDate |}

                Ok [ TaskAdded task ]

        | RemoveTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some _ ->
                let task = {| taskId = command.taskId |}
                Ok [ TaskRemoved task ]
            | None -> Ok []

        | ClearAllTasks ->
            match state.tasks |> Map.isEmpty with
            | true -> Ok []
            | false -> Ok [ AllTasksCleared ]

        | CompleteTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                let task = {| taskId = command.taskId |}
                Ok [ TaskCompleted task ]
            | Some _ -> Ok []
            | None -> completeTaskIdDoesNotExists |> Error

        | PostponeTask command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task when not task.completed ->
                if task.dueDate = Some command.dueDate then
                    Ok []
                else
                    let task =
                        {| taskId = command.taskId
                           dueDate = command.dueDate |}

                    Ok [ TaskPostponed task ]
            | Some _ -> Ok []
            | None -> postponeTaskIdDoesNotExists |> Error

        | KeepTaskOpen command ->
            match state.tasks |> Map.tryFind command.taskId with
            | Some task ->
                if task.completed || task.dueDate = None then
                    Ok []
                else
                    let task = {| taskId = command.taskId |}
                    Ok [ TaskKeptOpen task ]
            | None -> keepTaskOpenIdDoesNotExists |> Error
