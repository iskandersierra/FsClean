namespace Todos.Domain.TodoList

open System

type Command =
    | AddTask of AddTask
    | RemoveTask of RemoveTask
    | ClearAllTasks
    | CompleteTask of CompleteTask
    | PostponeTask of PostponeTask
    | KeepTaskOpen of KeepTaskOpen

and AddTask =
    { taskId: int
      title: string
      dueDate: DateTime option }

and RemoveTask =
    { taskId: int }

and CompleteTask =
    { taskId: int }

and PostponeTask =
    { taskId: int
      dueDate: DateTime }

and KeepTaskOpen =
    { taskId: int }
