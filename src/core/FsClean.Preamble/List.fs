[<RequireQualifiedAccess>]
module FsClean.List

let tryMaxWithCompareBy compare keySelector source =
    let rec loop current source =
        match source with
        | [] -> current |> Option.map snd
        | head :: tail ->
            match current with
            | None -> loop (Some(keySelector head, head)) tail
            | Some (currentKey, _) ->
                let key = keySelector head

                if compare key currentKey > 0 then
                    loop (Some(key, head)) tail
                else
                    loop current tail

    loop None

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
