namespace Todos.Domain.TodoList

open System
open Validus

open FsClean.Domain

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
    
    let createWithNow now value =
        let now = max now MinValue
        let msg =
            let now = (now: DateTime).ToString("yyyy-MM-dd")
            $"must be greater than or equal to {now}"
        validate {
            let! value =
                Check.WithMessage.DateTime.greaterThanOrEqualTo
                    now
                    (fun field -> $"'{field}' {msg}")
                    Fields.DUE_DATE
                    value

            return TaskDueDate value
        }

    let create value = createWithNow MinValue value
