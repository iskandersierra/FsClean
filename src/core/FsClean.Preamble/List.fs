/// module List defines functions and types for working with lists.
[<RequireQualifiedAccess>]
module FsClean.List

/// tryMaxWithCompareBy returns the maximum element of the given list, using the given compare function to compare elements.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same maximum key, the first element is returned.
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

/// tryMaxWithCompare returns the maximum element of the given list, using the given compare function to compare elements.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same maximum key, the first element is returned.
let inline tryMaxWithCompare compare = tryMaxWithCompareBy compare id

/// tryMaxBy returns the maximum element of the given list, using the given key selector to extract a key from each element.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same maximum key, the first element is returned.
let inline tryMaxBy keySelector =
    tryMaxWithCompareBy Comparer.defaultOf keySelector

/// tryMax returns the maximum element of the given list.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same maximum key, the first element is returned.
let inline tryMax source =
    tryMaxWithCompare Comparer.defaultOf source

/// tryMinWithCompareBy returns the minimum element of the given list, using the given compare function to compare elements.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same minimum key, the first element is returned.
let inline tryMinWithCompareBy compare keySelector =
    tryMaxWithCompareBy (Comparer.inverse compare) keySelector

/// tryMinWithCompare returns the minimum element of the given list, using the given compare function to compare elements.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same minimum key, the first element is returned.
let inline tryMinWithCompare compare = tryMinWithCompareBy compare id

/// tryMinBy returns the minimum element of the given list, using the given key selector to extract a key from each element.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same minimum key, the first element is returned.
let inline tryMinBy keySelector =
    tryMinWithCompareBy Comparer.defaultOf keySelector

/// tryMin returns the minimum element of the given list.
/// If the list is empty, None is returned.
/// If the list contains multiple elements with the same minimum key, the first element is returned.
let inline tryMin source =
    tryMinWithCompare Comparer.defaultOf source
