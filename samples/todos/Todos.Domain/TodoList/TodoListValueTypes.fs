namespace Todos.Domain.TodoList

open System
open Validus

open FsClean

type TaskId = TaskId of value: string

module TaskId =
    let value (TaskId value) = value

    let create value =
        EntityId.create Fields.TASK_ID value
        |> Result.map TaskId

type TaskTitle = TaskTitle of value: string

module TaskTitle =
    [<Literal>]
    let MinLength = 1

    [<Literal>]
    let MaxLength = 80

    let value (TaskTitle value) = value

    let create value =
        LimitedString.create MinLength MaxLength Fields.TITLE value
        |> Result.map TaskTitle

type TaskDueDate = TaskDueDate of value: DateTime

module TaskDueDate =
    let MinValue =
        DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    let value (TaskDueDate value) = value

    let create value =
        validate {
            let! value =
                Check.WithMessage.DateTime.between
                    MinValue
                    DateTime.MaxValue
                    (sprintf "'%s' must be greater than or equal to 1970-01-01")
                    Fields.DUE_DATE
                    value

            return TaskDueDate value
        }
