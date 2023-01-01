﻿[<RequireQualifiedAccess>]
module FsClean.Async

open System.Threading.Tasks

let map fn m =
    async {
        let! x = m
        return fn x }

let toTask (m: Async<'a>) : Task<'a> =
    task { return! m }

let startAsTask (m: Async<'a>) : Task<'a> =
    Async.StartAsTask(m)

let startAsVoidTask (m: Async<_>) : Task =
    startAsTask m :> Task
