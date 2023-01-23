namespace Todos.Application.TodoList

open FsClean.Application.KeyValueStorage
open FsClean.Application.UseCases

open Todos.Domain.TodoList

type ITodoListKVStore = IKVStore<string, State>

type ITodoListAggregateUseCase = IUseCase<struct (string * Command), unit>
type ITodoListAddTaskDtoUseCase = IUseCase<struct (string * Command.AddTaskDto), unit>

module AggregateUseCase =
    module OnKVStorage =
        module Stateless =
            let create (definition: ITodoListAggregateDefinition) (store: ITodoListKVStore) =
                AggregateUseCase.OnKVStorage.Stateless.create definition store

        module Stateful =
            let createSync (definition: ITodoListAggregateDefinition) (store: ITodoListKVStore) =
                AggregateUseCase.OnKVStorage.Stateful.create definition store
