namespace Todos.Domain.TodoList

type TodoListEvent =
    | TaskAdded of
        {| taskId: TaskId
           title: TaskTitle
           dueDate: TaskDueDate option |}
    | TaskRemoved of {| taskId: TaskId |}
    | AllTasksCleared
    | TaskCompleted of {| taskId: TaskId |}
    | TaskPostponed of
        {| taskId: TaskId
           dueDate: TaskDueDate |}
    | TaskKeptOpen of {| taskId: TaskId |}
