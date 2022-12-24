module Todos.Business.TodoList.TodoListAggregateUseCase

open FsClean.Business.UseCases.AggregateUseCase

open Todos.Domain.TodoList

let definition =
    { applyEvent = TodoListState.apply
      executeCommand = TodoListAggregate.execute }

let create ct = createUseCase ct definition
