/// module Async defines functions and types for asynchronous programming with the Async type.
[<RequireQualifiedAccess>]
module FsClean.Async

open System.Threading.Tasks

/// ret is a synonym for Async.Return. It returns a computation that returns the given value.
let inline ret x = async.Return x

/// map returns a computation that applies the given function to the result of the given computation.
let map fn m =
    async {
        let! x = m
        return fn x
    }

/// bind returns a computation that applies the given function to the result of the given computation.
let bind fn m =
    async {
        let! x = m
        return! fn x
    }

/// toTask converts an Async computation to a typed Task.
let inline toTask (m: Async<'a>) : Task<'a> = task { return! m }
/// toVoidTask converts an Async computation to an untyped Task.
let inline toVoidTask (m: Async<_>) : Task = toTask m :> Task
/// ofTask converts a Task to an Async computation.
let inline ofTask (m: Task<'a>) : Async<'a> = Async.AwaitTask m
/// ofVoidTask converts a Task to an Async computation.
let inline ofVoidTask (m: Task) : Async<_> = Async.AwaitTask m

/// startAsTask starts an Async computation as a typed Task.
let inline startAsTask (m: Async<'a>) : Task<'a> = Async.StartAsTask(m)

/// startAsVoidTask starts an Async computation as an untyped Task.
let inline startAsVoidTask (m: Async<_>) : Task = startAsTask m :> Task

module Cancellable =
    open System.Threading

    /// startAsTask starts an Async computation as a typed Task.
    /// The task can be cancelled by using the given cancellation token.
    let inline startAsTask (token: CancellationToken) (m: Async<'a>) : Task<'a> =
        Async.StartAsTask(m, cancellationToken = token)

    /// startAsVoidTask starts an Async computation as an untyped Task.
    /// The task can be cancelled by using the given cancellation token.
    let inline startAsVoidTask (token: CancellationToken) (m: Async<_>) : Task =
        startAsTask token m :> Task
