[<RequireQualifiedAccess>]
module FsClean.Array

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

let inline tryMaxWithCompare compare = tryMaxWithCompareBy compare id

let inline tryMaxBy keySelector =
    tryMaxWithCompareBy Comparer.defaultOf keySelector

let inline tryMax source =
    tryMaxWithCompare Comparer.defaultOf source

let inline tryMinWithCompareBy compare keySelector =
    tryMaxWithCompareBy (Comparer.inverse compare) keySelector

let tryMinWithCompare compare = tryMinWithCompareBy compare id

let inline tryMinBy keySelector =
    tryMinWithCompareBy Comparer.defaultOf keySelector

let inline tryMin source =
    tryMinWithCompare Comparer.defaultOf source
