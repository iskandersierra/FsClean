[<RequireQualifiedAccess>]
module FsClean.EqualityComparer

open System.Collections.Generic

type Equals<'a> = 'a -> 'a -> bool
type Hash<'a> = 'a -> int

let ofFuncs equals hash =
    { new IEqualityComparer<_> with
        member __.Equals(x, y) = equals x y
        member __.GetHashCode x = hash x }

let toFuncs (comparer: IEqualityComparer<_>) =
    let equals x y = comparer.Equals(x, y)
    let hash x = comparer.GetHashCode x
    equals, hash

let defaultOf<'a> = EqualityComparer<'a>.Default
