namespace Todos.Domain.TodoList

open System

type State = { tasks: Map<int, TaskState> }

and TaskState =
    { taskId: int
      title: string
      dueDate: DateTime option
      completed: bool }

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
