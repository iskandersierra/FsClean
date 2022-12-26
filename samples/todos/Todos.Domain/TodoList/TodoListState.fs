namespace Todos.Domain.TodoList

open System

type TodoListState = { tasks: Map<TaskId, TaskState> }

and TaskState =
    { taskId: TaskId
      title: TaskTitle
      dueDate: TaskDueDate option
      completed: bool }

module TodoListState =
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

    type TodoListStateDto =
        { tasks: TaskStateDto list }

    and TaskStateDto =
        { taskId: int
          title: string
          dueDate: DateTime option
          completed: bool }

    let toDto (state: TodoListState) : TodoListStateDto =
        { tasks =
            state.tasks
            |> Map.toList
            |> List.map snd
            |> List.map (fun task ->
                { taskId = task.taskId |> TaskId.value
                  title = task.title |> TaskTitle.value
                  dueDate = task.dueDate |> Option.map TaskDueDate.value
                  completed = task.completed }) }
