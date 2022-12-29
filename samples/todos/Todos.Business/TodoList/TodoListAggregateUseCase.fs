module Todos.Business.TodoList.TodoListAggregateUseCase

open FsClean.Business.UseCases

open Todos.Domain.TodoList

let definition : AggregateUseCase.Definition<TodoListState, TodoListEvent, TodoListCommand> =
    { applyEvent = TodoListState.apply
      executeCommand = TodoListAggregate.execute }

let create ct = AggregateUseCase.create ct definition

let createStateless = AggregateUseCase.createStateless definition
