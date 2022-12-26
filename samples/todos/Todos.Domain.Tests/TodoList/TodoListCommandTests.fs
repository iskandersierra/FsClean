module Todos.Domain.TodoList.TodoListCommandTests

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
let ``TodoListCommand.createAddTask should validate the command`` dto =
    let result = TodoListCommand.createAddTask dto
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
let ``TodoListCommand.createRemoveTask should validate the command`` dto =
    let result = TodoListCommand.createRemoveTask dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return RemoveTask {| taskId = taskId |}
        }
    test <@ result = expected @>

[<Property>]
let ``TodoListCommand.createCompleteTask should validate the command`` dto =
    let result = TodoListCommand.createCompleteTask dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return CompleteTask {| taskId = taskId |}
        }
    test <@ result = expected @>

[<Property>]
let ``TodoListCommand.createPostponeTask should validate the command`` dto =
    let result = TodoListCommand.createPostponeTask dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            and! dueDate = TaskDueDate.create dto.dueDate

            return PostponeTask {| taskId = taskId; dueDate = dueDate |}
        }
    test <@ result = expected @>

[<Property>]
let ``TodoListCommand.createKeepTaskOpen should validate the command`` dto =
    let result = TodoListCommand.createKeepTaskOpen dto
    let expected =
        validate {
            let! taskId = TaskId.create dto.taskId
            return KeepTaskOpen {| taskId = taskId |}
        }
    test <@ result = expected @>
