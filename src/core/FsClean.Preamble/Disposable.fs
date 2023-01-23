/// module Disposable defines functions and types for working with disposable objects.
[<RequireQualifiedAccess>]
module FsClean.Disposable

open System
open System.Threading.Tasks

/// defer creates a IDisposable object that invokes the given function when disposed.
let defer fn =
    let mutable disposed = false

    let dispose () =
        if not disposed then
            fn ()
            disposed <- true

    { new IDisposable with
        member __.Dispose() = dispose () }

/// deferTask creates a IAsyncDisposable object that invokes the given task when disposed.
let deferTask (fn: unit -> Task) =
    let mutable disposed = false

    let dispose () =
        task {
            if not disposed then
                do! fn ()
                disposed <- true
        }

    { new IAsyncDisposable with
        member __.DisposeAsync() = ValueTask(dispose ()) }

/// deferAsync creates a IAsyncDisposable object that invokes the given async computation when disposed.
let deferAsync (fn: unit -> Async<_>) = deferTask (fn >> Async.startAsVoidTask)
