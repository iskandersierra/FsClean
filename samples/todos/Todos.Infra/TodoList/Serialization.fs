namespace Todos.Infra.TodoList

open System

open FsClean
open FsClean.Application.KeyValueStorage
open FsToolkit.ErrorHandling

open Todos.Domain.TodoList

module AggregateUseCase =
    module Serialization =
        module V1 =
            [<CLIMutable>]
            type TaskSnapshot =
                { taskId: int
                  title: string
                  dueDate: DateTime Nullable
                  completed: bool }

            [<CLIMutable>]
            type Snapshot = { tasks: TaskSnapshot [] }

            let toTaskSnapshot (task: TaskState) =
                { taskId = TaskId.toValue task.taskId
                  title = TaskTitle.toValue task.title
                  dueDate =
                    task.dueDate
                    |> Option.map TaskDueDate.toValue
                    |> Option.toNullable
                  completed = task.completed }

            let ofTaskSnapshot (task: TaskSnapshot) =
                { TaskState.taskId = TaskId.ofValue task.taskId
                  title = TaskTitle.ofValue task.title
                  dueDate =
                    task.dueDate
                    |> Option.ofNullable
                    |> Option.map TaskDueDate.ofValue
                  completed = task.completed }

            let toSnapshot (state: State) : Snapshot =
                { tasks =
                    state.tasks
                    |> Map.toSeq
                    |> Seq.map (fun (_, t) -> toTaskSnapshot t)
                    |> Seq.toArray }

            let ofSnapshot (dto: Snapshot) : State =
                { tasks =
                    dto.tasks
                    |> Seq.map ofTaskSnapshot
                    |> Seq.map (fun t -> t.taskId, t)
                    |> Map.ofSeq }

            let converter: DualFn<State, Snapshot> =
                { forward = toSnapshot
                  backward = ofSnapshot }

        let versions =
            let toJson () =
                DualFn.create Json.serialize Json.deserialize

            [ "_V1", V1.converter |> DualFn.pipeTo (toJson ()) ]

        let stateKVStore store =
            KVStore.versionedStringKey versions store
