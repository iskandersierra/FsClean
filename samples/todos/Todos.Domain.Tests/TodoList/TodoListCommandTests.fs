module Todos.Domain.TodoList.CommandTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsClean.Domain
open FsClean.Domain.ValueTypesTests
open Validus

open Todos.Domain.TodoList

[<Property>]
let ``Command.createAddTask should validate the command`` dto =
    let result = Command.createAddTask dto
    let expected =
        validate {
            let! title = TaskTitle.create dto.title
            and! dueDate = ValueTypes.createOptional TaskDueDate.create dto.dueDate

            return
                AddTask
                    {| title = title
                       dueDate = dueDate |}
        }
    test <@ result = expected @>

[<Property>]
let ``Command.createRemoveTask should validate the command`` dto =
    let result = Command.createRemoveTask dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return RemoveTask {| taskId = taskId |}
        }
    test <@ result = expected @>

[<Property>]
let ``Command.createCompleteTask should validate the command`` dto =
    let result = Command.createCompleteTask dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return CompleteTask {| taskId = taskId |}
        }
    test <@ result = expected @>

[<Property>]
let ``Command.createPostponeTask should validate the command`` now dto =
    let result = Command.createPostponeTask now dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            and! dueDate = TaskDueDate.createWithNow now dto.dueDate

            return PostponeTask {| taskId = taskId; dueDate = dueDate |}
        }
    test <@ result = expected @>

[<Property>]
let ``Command.createKeepTaskOpen should validate the command`` dto =
    let result = Command.createKeepTaskOpen dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return KeepTaskOpen {| taskId = taskId |}
        }
    test <@ result = expected @>
