/// module Array defines functions and types for working with arrays.
[<RequireQualifiedAccess>]
module FsClean.Array

/// tryMaxWithCompareBy returns the maximum element of the given array, using the given compare function to compare elements.
let tryMaxWithCompareBy compare keySelector source =
    match Array.length source with
    | 0 -> None
    | n ->
        let mutable current = keySelector source.[0], source.[0]
        for i = 1 to n - 1 do
            let key = keySelector source.[i]
            if compare key (fst current) > 0 then
                current <- key, source.[i]
        Some(snd current)

/// tryMaxWithCompare returns the maximum element of the given array, using the given compare function to compare elements.
let inline tryMaxWithCompare compare = tryMaxWithCompareBy compare id

/// tryMaxBy returns the maximum element of the given array, using the given key selector to extract a key from each element.
let inline tryMaxBy keySelector =
    tryMaxWithCompareBy Comparer.defaultOf keySelector

/// tryMax returns the maximum element of the given array.
let inline tryMax source =
    tryMaxWithCompare Comparer.defaultOf source

/// tryMinWithCompareBy returns the minimum element of the given array, using the given compare function to compare elements.
let inline tryMinWithCompareBy compare keySelector =
    tryMaxWithCompareBy (Comparer.inverse compare) keySelector

/// tryMinWithCompare returns the minimum element of the given array, using the given compare function to compare elements.
let inline tryMinWithCompare compare = tryMinWithCompareBy compare id

/// tryMinBy returns the minimum element of the given array, using the given key selector to extract a key from each element.
let inline tryMinBy keySelector =
    tryMinWithCompareBy Comparer.defaultOf keySelector

/// tryMin returns the minimum element of the given array.
let inline tryMin source =
    tryMinWithCompare Comparer.defaultOf source
