[<RequireQualifiedAccess>]
module FsClean.Seq

let tryMaxWithCompareBy compare keySelector source =
    use enumerator = (source: _ seq).GetEnumerator()

    let rec loop current =
        match enumerator.MoveNext() with
        | false -> current |> Option.map snd
        | true ->
            match current with
            | None -> loop (Some(keySelector enumerator.Current, enumerator.Current))
            | Some (currentKey, _) ->
                let key = keySelector enumerator.Current

                if compare key currentKey > 0 then
                    loop (Some(key, enumerator.Current))
                else
                    loop current

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
