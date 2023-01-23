namespace Todos.Application.TodoList

open FsClean
open FsClean.Application.UseCases
open FsToolkit.ErrorHandling

open Todos.Domain.TodoList

type ITodoListAggregateDefinition = IAggregateDefinition<State, Event, Command>

module AggregateDefinition =
    let definition : ITodoListAggregateDefinition =
        AggregateDefinition.forDomain
            (konst State.init)
            State.apply
            (fun s -> Aggregate.execute s >> Result.map List.toSeq)
