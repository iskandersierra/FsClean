[<RequireQualifiedAccess>]
module FsClean.Seq

open System.Collections.Generic

let getEnumerator (source: 'a seq) = source.GetEnumerator()
let moveNext (enumerator: IEnumerator<'a>) = enumerator.MoveNext()
let getCurrent (enumerator: IEnumerator<'a>) = enumerator.Current

let tryMaxWithCompareBy compare keySelector =
    Seq.fold
        (fun acc x ->
            match acc with
            | None -> Some x
            | Some y ->
                match compare (keySelector x) (keySelector y) with
                | 1 -> Some x
                | _ -> acc)
        None

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

let scanCond fn acc source =
    let rec loop e acc =
        seq {
            match moveNext e with
            | true ->
                let current = getCurrent e

                match fn acc current with
                | Continue -> yield! loop e acc
                | ContinueWith acc ->
                    yield acc
                    yield! loop e acc
                | Break -> ()
                | BreakWith acc -> yield! acc
            | false -> ()
        }

    seq {
        use e = getEnumerator source
        yield acc
        yield! loop e acc
    }


let foldCond fn acc source =
    let rec loop e acc =
        match moveNext e with
        | true ->
            let current = getCurrent e

            match fn acc current with
            | Continue -> loop e acc
            | ContinueWith acc -> loop e acc
            | Break -> acc
            | BreakWith acc -> acc
        | false -> acc

    use e = getEnumerator source
    loop e acc
