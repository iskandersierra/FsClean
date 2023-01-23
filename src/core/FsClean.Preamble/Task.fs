/// module Task defines functions and types for asynchronous programming with the Task type.
[<RequireQualifiedAccess>]
module FsClean.Task

open System.Threading.Tasks

/// ret is a synonym for Task.FromResult. It returns a task computation that returns the given value.
let inline ret x = Task.FromResult x

/// map returns a task computation that applies the given function to the result of the given computation.
let map fn m =
    task {
        let! x = m
        return fn x
    }

/// bind returns a task computation that applies the given function to the result of the given computation.
let bind fn m =
    task {
        let! x = m
        return! fn x
    }

/// toAsync converts a Task computation to an Async computation.
let inline toAsync (m: Task<'a>) : Async<'a> = Async.AwaitTask m
/// toUnitAsync converts a Task computation to an Async computation.
let inline toUnitAsync (m: Task) : Async<_> = Async.AwaitTask m
/// ofAsync converts an Async computation to a Task computation.
let inline ofAsync (m: Async<'a>) : Task<'a> = task { return! m }
/// ofUnitAsync converts an Async computation to a Task computation.
let inline ofUnitAsync (m: Async<_>) : Task = ofAsync m :> Task
