namespace FsClean.Application

open System

type GetDateTime = unit -> DateTime

type Clock = { utcNow: GetDateTime }

module Clock =
    let system = { utcNow = fun () -> DateTime.UtcNow }

    let brokenAt date = { utcNow = fun () -> date }
