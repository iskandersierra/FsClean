[<RequireQualifiedAccess>]
module FsClean.Comparer

open System.Collections.Generic

open FsClean

type Compare<'a> = 'a -> 'a -> int

let ofFunc (compare: Compare<'a>) =
    { new IComparer<_> with
        member __.Compare(x, y) = compare x y }

let toFunc (comparer: IComparer<_>) =
    let compare x y = comparer.Compare(x, y)
    compare

let defaultComparerOf<'a> = Comparer<'a>.Default

let defaultOf<'a when 'a: comparison> : Compare<'a> =
    fun a b -> defaultComparerOf<'a>.Compare (a, b)

let inverse (compare: Compare<_>) = flip compare

let inverseComparer (comparer: IComparer<_>) = comparer |> toFunc |> inverse |> ofFunc
