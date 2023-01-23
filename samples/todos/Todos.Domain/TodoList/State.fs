﻿namespace Todos.Domain.TodoList

open System

type State = { tasks: Map<TaskId, TaskState> }

and TaskState =
    { taskId: TaskId
      title: TaskTitle
      dueDate: TaskDueDate option
      completed: bool }

[<RequireQualifiedAccess>]
module State =
    let init = { tasks = Map.empty }

    let apply state =
        function
        | TaskAdded event ->
            let todoTask =
                { taskId = event.taskId
                  title = event.title
                  dueDate = event.dueDate
                  completed = false }

            { state with tasks = state.tasks |> Map.add event.taskId todoTask }

        | TaskRemoved event -> { state with tasks = state.tasks |> Map.remove event.taskId }

        | AllTasksCleared -> { state with tasks = Map.empty }

        | TaskCompleted event ->
            match state.tasks |> Map.tryFind event.taskId with
            | Some todoTask ->
                { state with
                    tasks =
                        state.tasks
                        |> Map.add event.taskId { todoTask with completed = true } }
            | None -> state

        | TaskPostponed event ->
            match state.tasks |> Map.tryFind event.taskId with
            | Some todoTask ->
                { state with
                    tasks =
                        state.tasks
                        |> Map.add event.taskId { todoTask with dueDate = Some event.dueDate } }
            | None -> state

        | TaskKeptOpen event ->
            match state.tasks |> Map.tryFind event.taskId with
            | Some todoTask ->
                { state with
                    tasks =
                        state.tasks
                        |> Map.add event.taskId { todoTask with dueDate = None } }
            | None -> state

    type Dto =
        { tasks: TaskDto list }

    and TaskDto =
        { taskId: int
          title: string
          dueDate: DateTime option
          completed: bool }

    let toDto (state: State) : Dto =
        { tasks =
            state.tasks
            |> Map.toList
            |> List.map snd
            |> List.map (fun task ->
                { taskId = TaskId.toValue task.taskId
                  title = TaskTitle.toValue task.title
                  dueDate = task.dueDate |> Option.map TaskDueDate.toValue
                  completed = task.completed }) }

    let ofDto (dto: Dto) : State =
        { tasks =
            dto.tasks
            |> List.map (fun task ->
                TaskId.ofValue task.taskId,
                { TaskState.taskId = TaskId.ofValue task.taskId
                  title = TaskTitle.ofValue task.title
                  dueDate = task.dueDate |> Option.map TaskDueDate.ofValue
                  completed = task.completed })
            |> Map.ofList }
