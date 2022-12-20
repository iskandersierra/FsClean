namespace Todos.Domain.TodoList

open System
open Validus

type Command =
    | AddTask of AddTask
    | RemoveTask of RemoveTask
    | ClearAllTasks
    | CompleteTask of CompleteTask
    | PostponeTask of PostponeTask
    | KeepTaskOpen of KeepTaskOpen

and AddTask =
    { taskId: TaskId
      title: TaskTitle
      dueDate: TaskDueDate option }
    static member create taskId title dueDate =
        validate {
            let! taskId = TaskId.create taskId
            and! title = TaskTitle.create title

            and! dueDate =
                match dueDate with
                | Some dueDate -> TaskDueDate.create dueDate |> Result.map Some
                | None -> Ok None

            return
                AddTask
                    { taskId = taskId
                      title = title
                      dueDate = dueDate }
        }

and RemoveTask =
    { taskId: TaskId }
    static member create taskId =
        validate {
            let! taskId = TaskId.create taskId
            return RemoveTask { taskId = taskId }
        }

and CompleteTask =
    { taskId: TaskId }
    static member create taskId =
        validate {
            let! taskId = TaskId.create taskId
            return CompleteTask { taskId = taskId }
        }

and PostponeTask =
    { taskId: TaskId
      dueDate: TaskDueDate }
    static member create taskId dueDate =
        validate {
            let! taskId = TaskId.create taskId
            and! dueDate = TaskDueDate.create dueDate
            return PostponeTask { taskId = taskId; dueDate = dueDate }
        }

and KeepTaskOpen =
    { taskId: TaskId }
    static member create taskId =
        validate {
            let! taskId = TaskId.create taskId
            return KeepTaskOpen { taskId = taskId }
        }
