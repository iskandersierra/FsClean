module Todos.Domain.TodoList.TodoListStateTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open Validus

open FsClean.Domain
open FsClean.Domain.ValueTypesTests
open Todos.Domain.TodoList

[<Fact>]
let ``TodoListState.init should return the empty state`` () =
    let result = TodoListState.init
    let expected: TodoListState = { tasks = Map.empty }
    test <@ result = expected @>

[<Property>]
let ``TodoListState.apply TaskAdded should return a state with added task`` state event =
    let state' =
        TodoListState.apply state (TaskAdded event)

    let task = state'.tasks |> Map.find event.taskId

    let expectedTask =
        { taskId = event.taskId
          title = event.title
          dueDate = event.dueDate
          completed = false }

    test <@ task = expectedTask @>

[<Property>]
let ``TodoListState.apply TaskRemoved should return a state with removed task`` state event =
    let state' =
        TodoListState.apply state (TaskRemoved event)

    let result = state'.tasks |> Map.tryFind event.taskId
    test <@ result = None @>

[<Property>]
let ``TodoListState.apply AllTasksCleared should return a state with empty tasks`` state =
    let state' =
        TodoListState.apply state AllTasksCleared

    let result = state'.tasks
    test <@ result = Map.empty @>

[<Property>]
let ``TodoListState.apply TaskCompleted on non-existing task should return same state`` state =
    let state' =
        TodoListState.apply state (TaskCompleted {| taskId = TaskId(1234) |})

    test <@ state' = state @>

[<Property>]
let ``TodoListState.apply TaskCompleted on existing task should return a state with completed task`` state =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head
        let event = TaskCompleted {| taskId = taskId |}
        let state' = TodoListState.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.completed = true @>

[<Property>]
let ``TodoListState.apply TaskPostponed on non-existing task should return same state`` state dueDate =
    let state' =
        TodoListState.apply
            state
            (TaskPostponed
                {| taskId = TaskId(1234)
                   dueDate = dueDate |})

    test <@ state' = state @>

[<Property>]
let ``TodoListState.apply TaskPostponed on existing task should return a state with postponed task`` state dueDate =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head

        let event =
            TaskPostponed {| taskId = taskId; dueDate = dueDate |}

        let state' = TodoListState.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.dueDate = Some dueDate @>

[<Property>]
let ``TodoListState.apply TaskKeptOpen on non-existing task should return same state`` state =
    let state' =
        TodoListState.apply
            state
            (TaskKeptOpen {| taskId = TaskId(1234) |})

    test <@ state' = state @>

[<Property>]
let ``TodoListState.apply TaskKeptOpen on existing task should return a state with kept open task`` state =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head
        let event = TaskKeptOpen {| taskId = taskId |}
        let state' = TodoListState.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.dueDate = None @>
