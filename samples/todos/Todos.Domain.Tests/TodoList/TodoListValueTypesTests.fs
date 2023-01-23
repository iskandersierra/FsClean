module Todos.Domain.TodoList.TodoListValueTypesTests

open Swensen.Unquote
open FsCheck.Xunit
open FsClean.Domain
open FsClean.Domain.ValueTypesTests
open Validus

open Todos.Domain.TodoList


[<Property>]
let ``TaskId.value should return the value`` value =
    let taskId = TaskId.ofValue value
    test <@ TaskId.toValue taskId = value @>

[<Property>]
let ``TaskId.create should return a validated TaskId`` value =
    let actual = TaskId.create value
    let expected =
        validate {
            let! value = Check.Int.greaterThan 0 Fields.TASK_ID value
            return TaskId.ofValue value
        }
    test <@ actual = expected @>

[<Property>]
let ``TaskTitle.value should return the value`` value =
    let taskTitle = TaskTitle.ofValue value
    test <@ TaskTitle.toValue taskTitle = value @>

[<Property>]
let ``TaskTitle.create should return a validated TaskTitle`` value =
    TaskTitle.create value
    |> LimitedString.testIsValid TaskTitle.ofValue TaskTitle.MinLength TaskTitle.MaxLength Fields.TITLE value

[<Property>]
let ``TaskDueDate.value should return the value`` value =
    let taskDueDate = TaskDueDate.ofValue value
    test <@ TaskDueDate.toValue taskDueDate = value @>

[<Property>]
let ``TaskDueDate.create should return a validated TaskDueDate`` value =
    let result = TaskDueDate.create value

    let expected =
        if value < TaskDueDate.MinValue then
            Error(ValidationErrors.create Fields.DUE_DATE [ $"'{Fields.DUE_DATE}' must be greater than or equal to 1970-01-01" ])
        else
            Ok(TaskDueDate.ofValue value)

    test <@ result = expected @>
