namespace Todos.Domain.TodoList

open System
open Validus

type TaskId =
    | TaskId of value: string
    member this.Value =
        match this with
        | TaskId value -> value

    static member create value =
        validate {
            let! value = Check.String.notEmpty value Fields.TASK_ID
            let! value = Check.String.betweenLen 1 100 value Fields.TASK_ID
            return TaskId value
        }

type TaskTitle =
    | TaskTitle of value: string
    member this.Value =
        match this with
        | TaskTitle value -> value

    static member create value =
        validate {
            let! value = Check.String.notEmpty value Fields.TITLE
            let! value = Check.String.betweenLen 1 100 value Fields.TITLE
            return TaskTitle value
        }

type TaskDueDate =
    | TaskDueDate of value: DateTime
    member this.Value =
        match this with
        | TaskDueDate value -> value

    static member create value =
        validate {
            let! value = Check.DateTime.notEquals DateTime.MinValue Fields.DUE_DATE value
            return TaskDueDate value
        }
