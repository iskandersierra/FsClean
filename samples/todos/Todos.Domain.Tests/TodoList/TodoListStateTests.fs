module Todos.Domain.TodoList.StateTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsClean.Domain
open FsClean.Domain.ValueTypesTests
open Validus

open Todos.Domain.TodoList

[<Fact>]
let ``State.init should return the empty state`` () =
    let result = State.init
    let expected: State = { tasks = Map.empty }
    test <@ result = expected @>

[<Property>]
let ``State.apply TaskAdded should return a state with added task`` state event =
    let state' =
        State.apply state (TaskAdded event)

    let task = state'.tasks |> Map.find event.taskId

    let expectedTask =
        { taskId = event.taskId
          title = event.title
          dueDate = event.dueDate
          completed = false }

    test <@ task = expectedTask @>

[<Property>]
let ``State.apply TaskRemoved should return a state with removed task`` state event =
    let state' =
        State.apply state (TaskRemoved event)

    let result = state'.tasks |> Map.tryFind event.taskId
    test <@ result = None @>

[<Property>]
let ``State.apply AllTasksCleared should return a state with empty tasks`` state =
    let state' =
        State.apply state AllTasksCleared

    let result = state'.tasks
    test <@ result = Map.empty @>

[<Property>]
let ``State.apply TaskCompleted on non-existing task should return same state`` state =
    let state' =
        State.apply state (TaskCompleted {| taskId = TaskId.ofValue 1234 |})

    test <@ state' = state @>

[<Property>]
let ``State.apply TaskCompleted on existing task should return a state with completed task`` state =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head
        let event = TaskCompleted {| taskId = taskId |}
        let state' = State.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.completed = true @>

[<Property>]
let ``State.apply TaskPostponed on non-existing task should return same state`` state dueDate =
    let state' =
        State.apply
            state
            (TaskPostponed
                {| taskId = TaskId.ofValue 1234
                   dueDate = dueDate |})

    test <@ state' = state @>

[<Property>]
let ``State.apply TaskPostponed on existing task should return a state with postponed task`` state dueDate =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head

        let event =
            TaskPostponed {| taskId = taskId; dueDate = dueDate |}

        let state' = State.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.dueDate = Some dueDate @>

[<Property>]
let ``State.apply TaskKeptOpen on non-existing task should return same state`` state =
    let state' =
        State.apply
            state
            (TaskKeptOpen {| taskId = TaskId.ofValue 1234 |})

    test <@ state' = state @>

[<Property>]
let ``State.apply TaskKeptOpen on existing task should return a state with kept open task`` state =
    if state.tasks |> Map.isEmpty then
        test <@ true @>
    else
        let taskId = state.tasks |> Map.keys |> Seq.head
        let event = TaskKeptOpen {| taskId = taskId |}
        let state' = State.apply state event
        let task = state'.tasks |> Map.find taskId
        test <@ task.dueDate = None @>
