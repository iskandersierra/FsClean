module FsClean.AsyncTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit

open FsClean

[<Property>]
let ``Async.map should map the async result with the given function`` (x: int) =
    let m = async { return x }
    let fn = (+) 1
    let expected = fn x
    let actual = Async.map fn m |> Async.RunSynchronously
    test <@ actual = expected @>

[<Property>]
let ``Async.toTask should convert an async into a task`` (x: int) =
    let m = async { return x }
    let expected = x
    let actual = (Async.toTask m).GetAwaiter().GetResult()
    test <@ actual = expected @>
