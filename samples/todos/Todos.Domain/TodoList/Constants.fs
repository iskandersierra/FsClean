namespace Todos.Domain.TodoList

[<AutoOpen>]
module TodoListEntity =
    let ENTITY_NAME = "todo_list"

    module Operations =
        let ADD_TASK = "add_task"
        let REMOVE_TASK = "remove_task"
        let CLEAR_ALL_TASKS = "clear_all_tasks"
        let COMPLETE_TASK = "complete_task"
        let POSTPONE_TASK = "postpone_task"
        let KEEP_TASK_OPEN = "keep_task_open"

    module Fields =
        let TASK_ID = "task_id"
        let TITLE = "title"
        let DUE_DATE = "due_date"
        let COMPLETED = "completed"

    module Errors =
        open FsClean
        let TASK_ID_ALREADY_EXISTS = $"{ENTITY_NAME}.task_id_already_exists"
        let TASK_ID_DOES_NOT_EXISTS = $"{ENTITY_NAME}.task_id_does_not_exists"
        let TASK_ALREADY_COMPLETED = $"{ENTITY_NAME}.task_already_completed"

        let TaskIdAlreadyExistsConflict =
            Map.empty
            |> DomainError.conflict ENTITY_NAME
            |> DomainError.setCode TASK_ID_ALREADY_EXISTS
            |> DomainError.setDescription "Task ID already exists"

        let TaskIdDoesNotExistsConflict =
            Map.empty
            |> DomainError.conflict ENTITY_NAME
            |> DomainError.setCode TASK_ID_DOES_NOT_EXISTS
            |> DomainError.setDescription "Task ID does not exists"

        let TaskAlreadyCompletedConflict =
            Map.empty
            |> DomainError.conflict ENTITY_NAME
            |> DomainError.setCode TASK_ALREADY_COMPLETED
            |> DomainError.setDescription "Task already completed"
