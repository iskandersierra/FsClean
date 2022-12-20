namespace Todos.Domain.TodoList

open System
open Validus

type Command =
    | AddTask of
        {| taskId: TaskId
           title: TaskTitle
           dueDate: TaskDueDate option |}
    | RemoveTask of {| taskId: TaskId |}
    | ClearAllTasks
    | CompleteTask of {| taskId: TaskId |}
    | PostponeTask of
        {| taskId: TaskId
           dueDate: TaskDueDate |}
    | KeepTaskOpen of {| taskId: TaskId |}

module Command =
    type AddTaskDto =
        { taskId: string
          title: string
          dueDate: DateTime option }

    type RemoveTaskDto = { taskId: string }

    type CompleteTaskDto = { taskId: string }

    type PostponeTaskDto = { taskId: string; dueDate: DateTime }

    type KeepTaskOpenDto = { taskId: string }

    let createAddTask (dto: AddTaskDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            and! title = TaskTitle.create dto.title

            and! dueDate =
                match dto.dueDate with
                | Some dueDate -> TaskDueDate.create dueDate |> Result.map Some
                | None -> Ok None

            return
                AddTask
                    {| taskId = taskId
                       title = title
                       dueDate = dueDate |}
        }

    let createRemoveTask (dto: RemoveTaskDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            return RemoveTask {| taskId = taskId |}
        }

    let createCompleteTask (dto: CompleteTaskDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            return CompleteTask {| taskId = taskId |}
        }

    let createPostponeTask (dto: PostponeTaskDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            and! dueDate = TaskDueDate.create dto.dueDate
            return PostponeTask {| taskId = taskId; dueDate = dueDate |}
        }

    let createKeepTaskOpen (dto: KeepTaskOpenDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            return KeepTaskOpen {| taskId = taskId |}
        }
