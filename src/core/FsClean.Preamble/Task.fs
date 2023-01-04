[<RequireQualifiedAccess>]
module FsClean.Task

open System.Threading.Tasks

let map fn m =
    task {
        let! x = m
        return fn x
    }

let bind fn m =
    task {
        let! x = m
        return! fn x
    }

let toAsync (m: Task<'a>) : Async<'a> = Async.AwaitTask m
let ofAsync (m: Async<'a>) : Task<'a> = task { return! m }
