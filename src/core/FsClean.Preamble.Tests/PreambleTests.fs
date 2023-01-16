module FsClean.PreambleTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit
open FsClean

[<Property>]
let ``flip should flip function parameters`` (fn: int -> int -> int) (a: int) (b: int) =
    let expected = fn b a
    let actual = flip fn a b
    test <@ EqualityComparer.defaultOf<_>.Equals(actual, expected) @>
    test <@ actual = expected @>

[<Property>]
let ``tee should call function and return same result`` (a: int) =
    let mutable called = false
    let fn x = called <- true
    let expected = a
    let actual = tee fn a
    test <@ called = true @>
    test <@ actual = expected @>
