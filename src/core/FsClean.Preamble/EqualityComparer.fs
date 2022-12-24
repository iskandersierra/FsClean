[<RequireQualifiedAccess>]
module FsClean.EqualityComparer

open System.Collections.Generic

let ofFuncs equals hash =
    { new IEqualityComparer<_> with
        member __.Equals(x, y) = equals x y
        member __.GetHashCode x = hash x }

let defaultOf<'a> = EqualityComparer<'a>.Default
