module Todos.Domain.TodoList.TodoListConstantsTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit

open FsClean
open Todos.Domain.TodoList

[<Fact>]
let ``TODO_LIST_ENTITY_NAME should have proper value`` () =
    test <@ TODO_LIST_ENTITY_NAME = "todo_list" @>

[<Fact>]
let ``Operations.ADD_TASK should have proper value`` () =
    test <@ Operations.ADD_TASK = "add_task" @>

[<Fact>]
let ``Operations.REMOVE_TASK should have proper value`` () =
    test <@ Operations.REMOVE_TASK = "remove_task" @>

[<Fact>]
let ``Operations.CLEAR_ALL_TASKS should have proper value`` () =
    test <@ Operations.CLEAR_ALL_TASKS = "clear_all_tasks" @>

[<Fact>]
let ``Operations.COMPLETE_TASK should have proper value`` () =
    test <@ Operations.COMPLETE_TASK = "complete_task" @>

[<Fact>]
let ``Operations.POSTPONE_TASK should have proper value`` () =
    test <@ Operations.POSTPONE_TASK = "postpone_task" @>

[<Fact>]
let ``Operations.KEEP_TASK_OPEN should have proper value`` () =
    test <@ Operations.KEEP_TASK_OPEN = "keep_task_open" @>

[<Fact>]
let ``Fields.TASK_ID should have proper value`` () = test <@ Fields.TASK_ID = "task_id" @>

[<Fact>]
let ``Fields.TITLE should have proper value`` () = test <@ Fields.TITLE = "title" @>

[<Fact>]
let ``Fields.DUE_DATE should have proper value`` () = test <@ Fields.DUE_DATE = "due_date" @>

[<Fact>]
let ``Fields.COMPLETED should have proper value`` () =
    test <@ Fields.COMPLETED = "completed" @>

[<Fact>]
let ``Errors.TASK_ID_ALREADY_EXISTS should have proper value`` () =
    test <@ Errors.TASK_ID_ALREADY_EXISTS = "todo_list.task_id_already_exists" @>

[<Fact>]
let ``Errors.TASK_ID_DOES_NOT_EXISTS should have proper value`` () =
    test <@ Errors.TASK_ID_DOES_NOT_EXISTS = "todo_list.task_id_does_not_exists" @>

[<Fact>]
let ``Errors.TASK_ALREADY_COMPLETED should have proper value`` () =
    test <@ Errors.TASK_ALREADY_COMPLETED = "todo_list.task_already_completed" @>

[<Fact>]
let ``Errors.TaskIdAlreadyExistsConflict should have proper values`` () =
    let expected =
        { DomainError.conflict with
            code = Errors.TASK_ID_ALREADY_EXISTS
            entity = Some TODO_LIST_ENTITY_NAME
            description = "Task ID already exists" }

    test <@ Errors.TaskIdAlreadyExistsConflict = expected @>

[<Fact>]
let ``Errors.TaskIdDoesNotExistsConflict should have proper values`` () =
    let expected =
        { DomainError.conflict with
            code = Errors.TASK_ID_DOES_NOT_EXISTS
            entity = Some TODO_LIST_ENTITY_NAME
            description = "Task ID does not exists" }

    test <@ Errors.TaskIdDoesNotExistsConflict = expected @>

[<Fact>]
let ``Errors.TaskAlreadyCompletedConflict should have proper values`` () =
    let expected =
        { DomainError.conflict with
            code = Errors.TASK_ALREADY_COMPLETED
            entity = Some TODO_LIST_ENTITY_NAME
            description = "Task already completed" }

    test <@ Errors.TaskAlreadyCompletedConflict = expected @>

