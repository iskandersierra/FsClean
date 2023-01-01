[<RequireQualifiedAccess>]
module FsClean.Disposable

open System
open System.Threading.Tasks

let defer fn =
    let mutable disposed = false

    let dispose () =
        if not disposed then
            fn ()
            disposed <- true

    { new IDisposable with
        member __.Dispose() = dispose () }

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

let deferAsync (fn: unit -> Async<_>) = deferTask (fn >> Async.startAsVoidTask)
