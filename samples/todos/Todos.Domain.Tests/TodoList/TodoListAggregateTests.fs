module Todos.Domain.TodoList.TodoListAggregateTests

open System
open Xunit
open Swensen.Unquote

open Todos.Domain.TodoList

[<Fact>]
let ``TodoListAggregate.execute on AddTask should return events`` () =
    let command =
        AddTask
            {| title = TaskTitle.ofValue "title"
               dueDate = None |}

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    let expected =
        [ TaskAdded
              {| taskId = TaskId.ofValue 1
                 title = TaskTitle.ofValue "title"
                 dueDate = None |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on RemoveTask when task exists should return events`` () =
    let taskId = 1234

    let command =
        RemoveTask {| taskId = TaskId.ofValue taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = false } ] }

    let result = Aggregate.execute state command

    let expected =
        [ TaskRemoved {| taskId = TaskId.ofValue taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on RemoveTask when task does not exist should return no events`` () =
    let taskId = 1234

    let command =
        RemoveTask {| taskId = TaskId.ofValue taskId |}

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on ClearAllTasks when tasks exist should return events`` () =
    let taskId = 1234

    let command = ClearAllTasks

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = false } ] }

    let result = Aggregate.execute state command

    let expected = [ AllTasksCleared ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on ClearAllTasks when tasks do not exist should return no events`` () =
    let command = ClearAllTasks

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task exists should return events`` () =
    let taskId = 1234

    let command =
        CompleteTask {| taskId = TaskId.ofValue taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = false } ] }

    let result = Aggregate.execute state command

    let expected =
        [ TaskCompleted {| taskId = TaskId.ofValue taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task is already completed should return no events`` () =
    let taskId = 1234

    let command =
        CompleteTask {| taskId = TaskId.ofValue taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = true } ] }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on CompleteTask when task does not exist should return error`` () =
    let taskId = 1234

    let command =
        CompleteTask {| taskId = TaskId.ofValue taskId |}

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    let expected = Aggregate.completeTaskIdDoesNotExists

    test <@ result = Error expected @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task exists should return events`` () =
    let taskId = 1234

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId.ofValue taskId
               dueDate = TaskDueDate.ofValue newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = false } ] }

    let result = Aggregate.execute state command

    let expected =
        [ TaskPostponed
              {| taskId = TaskId.ofValue taskId
                 dueDate = TaskDueDate.ofValue newDate |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when dueDate is the same should return no events`` () =
    let taskId = 1234

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId.ofValue taskId
               dueDate = TaskDueDate.ofValue newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = Some(TaskDueDate.ofValue newDate)
                          completed = false } ] }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task is already completed should return no events`` () =
    let taskId = 1234

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId.ofValue taskId
               dueDate = TaskDueDate.ofValue newDate |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = None
                          completed = true } ] }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on PostponeTask when task does not exist should return error`` () =
    let taskId = 1234

    let newDate = DateTime.Now.AddDays 1.0

    let command =
        PostponeTask
            {| taskId = TaskId.ofValue taskId
               dueDate = TaskDueDate.ofValue newDate |}

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    let expected = Aggregate.postponeTaskIdDoesNotExists

    test <@ result = Error expected @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task exists should return events`` () =
    let taskId = 1234

    let command =
        KeepTaskOpen {| taskId = TaskId.ofValue taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = Some(TaskDueDate.ofValue (DateTime.Now.AddDays 1.0))
                          completed = false } ] }

    let result = Aggregate.execute state command

    let expected =
        [ TaskKeptOpen {| taskId = TaskId.ofValue taskId |} ]

    test <@ result = Ok expected @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task is already completed should return no events`` () =
    let taskId = 1234

    let command =
        KeepTaskOpen {| taskId = TaskId.ofValue taskId |}

    let state =
        { tasks =
            Map.ofSeq [ TaskId.ofValue taskId,
                        { taskId = TaskId.ofValue taskId
                          title = TaskTitle.ofValue "title"
                          dueDate = Some(TaskDueDate.ofValue (DateTime.Now.AddDays 1.0))
                          completed = true } ] }

    let result = Aggregate.execute state command

    test <@ result = Ok [] @>

[<Fact>]
let ``TodoListAggregate.execute on KeepTaskOpen when task does not exist should return error`` () =
    let taskId = 1234

    let command =
        KeepTaskOpen {| taskId = TaskId.ofValue taskId |}

    let state = { tasks = Map.empty }

    let result = Aggregate.execute state command

    let expected = Aggregate.keepTaskOpenIdDoesNotExists

    test <@ result = Error expected @>
