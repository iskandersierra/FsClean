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
    { taskId: TaskId
      title: TaskTitle
      dueDate: TaskDueDate option }

and TaskRemoved =
    { taskId: TaskId }

and TaskCompleted =
    { taskId: TaskId }

and TaskPostponed =
    { taskId: TaskId
      dueDate: TaskDueDate }

and TaskKeptOpen =
    { taskId: TaskId }
