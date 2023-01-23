/// module Comparer defines functions and types for working with comparers.
[<RequireQualifiedAccess>]
module FsClean.Comparer

open System.Collections.Generic

open FsClean

/// type Compare defines a function that compares two values for ordering.
type Compare<'a> = 'a -> 'a -> int

/// ofFunc creates a comparer from the given compare function.
let ofFunc (compare: Compare<'a>) =
    { new IComparer<_> with
        member __.Compare(x, y) = compare x y }

/// toFunc creates a compare function from the given comparer.
let toFunc (comparer: IComparer<_>) =
    let compare x y = comparer.Compare(x, y)
    compare

/// defaultComparerOf returns the default comparer for the given type.
let defaultComparerOf<'a> = Comparer<'a>.Default

/// defaultOf returns the default compare for the given type.
let defaultOf<'a when 'a: comparison> : Compare<'a> = toFunc defaultComparerOf<'a>

/// inverse returns a compare function that is the inverse of the given compare function.
let inverse (compare: Compare<_>) = flip compare

/// inverseComparer returns a comparer that is the inverse of the given comparer.
let inverseComparer (comparer: IComparer<_>) = comparer |> toFunc |> inverse |> ofFunc
