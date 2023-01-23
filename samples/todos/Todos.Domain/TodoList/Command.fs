namespace Todos.Domain.TodoList

open System

open FsClean.Domain
open Validus

type Command =
    | AddTask of
        {| title: TaskTitle
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
        { title: string
          dueDate: DateTime option }

    type RemoveTaskDto = { taskId: int }

    type CompleteTaskDto = { taskId: int }

    type PostponeTaskDto = { taskId: int; dueDate: DateTime }

    type KeepTaskOpenDto = { taskId: int }

    let createAddTask (dto: AddTaskDto) =
        validate {
            let! title = TaskTitle.create dto.title
            and! dueDate = ValueTypes.createOptional TaskDueDate.create dto.dueDate

            return
                AddTask
                    {| title = title
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

    let createPostponeTask now (dto: PostponeTaskDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            and! dueDate = TaskDueDate.createWithNow now dto.dueDate
            return PostponeTask {| taskId = taskId; dueDate = dueDate |}
        }

    let createKeepTaskOpen (dto: KeepTaskOpenDto) =
        validate {
            let! taskId = TaskId.create dto.taskId
            return KeepTaskOpen {| taskId = taskId |}
        }
