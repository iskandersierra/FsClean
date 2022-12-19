namespace Todos.Domain.TodoList

open System

type Event =
    | TaskAdded of TaskAdded
    | TaskRemoved of TaskRemoved
    | AllTasksCleared
    | TaskCompleted of TaskCompleted
    | TaskPostponed of TaskPostponed
    | TaskKeptOpen of TaskKeptOpen

and TaskAdded =
    { taskId: int
      title: string
      dueDate: DateTime option }

and TaskRemoved =
    { taskId: int }

and TaskCompleted =
    { taskId: int }

and TaskPostponed =
    { taskId: int
      dueDate: DateTime }

and TaskKeptOpen =
    { taskId: int }
