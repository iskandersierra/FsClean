module FsClean.EqualityComparerTests

open System
open System.Collections.Generic
open Xunit
open Swensen.Unquote
open FsCheck.Xunit

open FsClean

[<Fact>]
let ``defaultOf`` () =
    let actual = EqualityComparer.defaultOf<int>
    let expected = EqualityComparer<int>.Default
    test <@ actual = expected @>

[<Property>]
let ``ofFuncs`` (x: int) (y: int) =
    let comparer : IEqualityComparer<int> = EqualityComparer.ofFuncs (=) (fun x -> x)
    let actualEquals = comparer.Equals(x, y)
    let actualHash = comparer.GetHashCode(x)
    let expectedEquals = x = y
    let expectedHash = x
    test <@ actualEquals = expectedEquals @>
    test <@ actualHash = expectedHash @>
