﻿module Todos.Domain.TodoList.TodoListAggregateTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open Validus

open FsClean
open FsClean.ValueTypesTests
open Todos.Domain.TodoList

[<Fact>]
let ``TodoListAggregate.execute on AddTask when taskId already exists should return error`` () =
    let taskId = EntityId.newGuid ()

    let command =
        AddTask
            {| taskId = TaskId taskId
               title = TaskTitle "title"
               dueDate = None |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = false } ] }

    let result = TodoListAggregate.execute state command
    let expected = TodoListAggregate.addTaskIdAlreadyExists
    test <@ result = Error expected @>


[<Fact>]
let ``TodoListAggregate.execute on AddTask when task is new should return events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        AddTask
            {| taskId = TaskId taskId
               title = TaskTitle "title"
               dueDate = None |}

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command

    let expected =
        [ TaskAdded
              {| taskId = TaskId taskId
                 title = TaskTitle "title"
                 dueDate = None |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on RemoveTask when task exists should return events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        RemoveTask
            {| taskId = TaskId taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    let expected =
        [ TaskRemoved
              {| taskId = TaskId taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on RemoveTask when task does not exist should return no events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        RemoveTask
            {| taskId = TaskId taskId |}

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on ClearAllTasks when tasks exist should return events`` () =
    let taskId = EntityId.newGuid ()

    let command = ClearAllTasks

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    let expected = [ AllTasksCleared ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on ClearAllTasks when tasks do not exist should return no events`` () =
    let command = ClearAllTasks

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task exists should return events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        CompleteTask
            {| taskId = TaskId taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    let expected =
        [ TaskCompleted
              {| taskId = TaskId taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task is already completed should return no events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        CompleteTask
            {| taskId = TaskId taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = true } ] }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task does not exist should return error`` () =
    let taskId = EntityId.newGuid ()

    let command =
        CompleteTask
            {| taskId = TaskId taskId |}

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command
    let expected = TodoListAggregate.completeTaskIdDoesNotExists
    test <@ result = Error expected @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task exists should return events`` () =
    let taskId = EntityId.newGuid ()

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId taskId
               dueDate = TaskDueDate newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    let expected =
        [ TaskPostponed
              {| taskId = TaskId taskId
                 dueDate = TaskDueDate newDate |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when dueDate is the same should return no events`` () =
    let taskId = EntityId.newGuid ()

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId taskId
               dueDate = TaskDueDate newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = Some (TaskDueDate newDate)
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task is already completed should return no events`` () =
    let taskId = EntityId.newGuid ()

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId taskId
               dueDate = TaskDueDate newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = None
                          completed = true } ] }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task does not exist should return error`` () =
    let taskId = EntityId.newGuid ()

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId taskId
               dueDate = TaskDueDate newDate |}

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command
    let expected = TodoListAggregate.postponeTaskIdDoesNotExists
    test <@ result = Error expected @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task exists should return events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        KeepTaskOpen
            {| taskId = TaskId taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = Some (TaskDueDate (DateTime.Now.AddDays 1.0))
                          completed = false } ] }

    let result = TodoListAggregate.execute state command

    let expected =
        [ TaskKeptOpen
              {| taskId = TaskId taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task is already completed should return no events`` () =
    let taskId = EntityId.newGuid ()

    let command =
        KeepTaskOpen
            {| taskId = TaskId taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId taskId,
                        { taskId = TaskId taskId
                          title = TaskTitle "title"
                          dueDate = Some (TaskDueDate (DateTime.Now.AddDays 1.0))
                          completed = true } ] }

    let result = TodoListAggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task does not exist should return error`` () =
    let taskId = EntityId.newGuid ()

    let command =
        KeepTaskOpen
            {| taskId = TaskId taskId |}

    let state = { tasks = Map.empty }

    let result = TodoListAggregate.execute state command
    let expected = TodoListAggregate.keepTaskOpenIdDoesNotExists
    test <@ result = Error expected @>
