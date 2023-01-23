/// module EqualityComparer defines functions and types for working with equality comparers.
[<RequireQualifiedAccess>]
module FsClean.EqualityComparer

open System.Collections.Generic

/// type Equals defines a function that compares two values for equality.
type Equals<'a> = 'a -> 'a -> bool
/// type Hash defines a function that computes a hash code for a value.
type Hash<'a> = 'a -> int

/// ofFuncs creates an equality comparer from the given equals and hash functions.
let ofFuncs equals hash =
    { new IEqualityComparer<_> with
        member __.Equals(x, y) = equals x y
        member __.GetHashCode x = hash x }

/// toFuncs creates an equals and hash function pair from the given equality comparer.
let toFuncs (comparer: IEqualityComparer<_>) =
    let equals x y = comparer.Equals(x, y)
    let hash x = comparer.GetHashCode x
    equals, hash

/// defaultOf returns the default equality comparer for the given type.
let defaultOf<'a> = EqualityComparer<'a>.Default
