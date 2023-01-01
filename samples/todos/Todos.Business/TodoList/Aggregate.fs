module Todos.Business.TodoList.Aggregate

open FsClean
open FsClean.Business.UseCases

open Todos.Domain.TodoList

let definition: Aggregate.Definition<_, _, _> =
    { initialState = konst State.init
      applyEvent = State.apply
      executeCommand = Aggregate.execute }

module Stateless =
    let create =
        Aggregate.Stateless.create definition
