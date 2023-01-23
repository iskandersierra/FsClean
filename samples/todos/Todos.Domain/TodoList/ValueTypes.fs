namespace Todos.Domain.TodoList

open System

open FsClean.Domain
open FSharp.UMX
open Validus

type TaskId = int<taskId>
and [<Measure>] taskId

module TaskId =
    let ofValue (value: int) : TaskId = UMX.tag value
    let toValue (value: TaskId) : int = UMX.untag value
    let create value : ValidationResult<TaskId> =
        validate {
            let! value = Check.Int.greaterThan 0 Fields.TASK_ID value
            return UMX.tag value
        }

type TaskTitle = string<taskTitle>
and [<Measure>] taskTitle

module TaskTitle =
    [<Literal>]
    let MinLength = 1

    [<Literal>]
    let MaxLength = 80

    let ofValue (value: string) : TaskTitle = UMX.tag value
    let toValue (value: TaskTitle) : string = UMX.untag value
    let create value : ValidationResult<TaskTitle> =
        validate {
            let! value = LimitedString.create MinLength MaxLength Fields.TITLE value
            return UMX.tag value
        }

type TaskDueDate = DateTime<taskDueDate>
and [<Measure>] taskDueDate

module TaskDueDate =
    let MinValue =
        DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    let ofValue (value: DateTime) : TaskDueDate = UMX.tag value
    let toValue (value: TaskDueDate) : DateTime = UMX.untag value

    let createWithNow now value : ValidationResult<TaskDueDate> =
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

            return UMX.tag value
        }

    let create value = createWithNow MinValue value
