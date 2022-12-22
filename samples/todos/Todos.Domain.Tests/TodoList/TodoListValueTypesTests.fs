module Todos.Domain.TodoList.TodoListValueTypesTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open Validus

open FsClean.Domain
open FsClean.Domain.ValueTypesTests
open Todos.Domain.TodoList


[<Property>]
let ``TaskId.value should return the value`` value =
    let taskId = TaskId value
    test <@ TaskId.value taskId = value @>

[<Property>]
let ``TaskId.create should return a validated TaskId`` value =
    TaskId.create value
    |> EntityId.testIsValid TaskId Fields.TASK_ID value

[<Property>]
let ``TaskTitle.value should return the value`` value =
    let taskTitle = TaskTitle value
    test <@ TaskTitle.value taskTitle = value @>

[<Property>]
let ``TaskTitle.create should return a validated TaskTitle`` value =
    TaskTitle.create value
    |> LimitedString.testIsValid TaskTitle TaskTitle.MinLength TaskTitle.MaxLength Fields.TITLE value

[<Property>]
let ``TaskDueDate.value should return the value`` value =
    let taskDueDate = TaskDueDate value
    test <@ TaskDueDate.value taskDueDate = value @>

[<Property>]
let ``TaskDueDate.create should return a validated TaskDueDate`` value =
    let result = TaskDueDate.create value

    let expected =
        if value < TaskDueDate.MinValue then
            Error(ValidationErrors.create Fields.DUE_DATE [ $"'{Fields.DUE_DATE}' must be greater than or equal to 1970-01-01" ])
        else
            Ok(TaskDueDate value)

    test <@ result = expected @>
