namespace FsClean

/// FoldCondition is used to control the behavior of the foldCond/scanCond functions.
/// Continue: continue folding/scaning without yielding a new value.
/// ContinueWith: continue folding/scaning and yield a new value.
/// Break: stop folding/scaning without yielding a new value.
/// BreakWith: stop folding/scaning and yield a last value.
type FoldCondition<'a> =
    | Continue
    | ContinueWith of 'a
    | Break
    | BreakWith of 'a

/// module Seq defines functions and types for working with sequences.
[<RequireQualifiedAccess>]
module Seq =
    open System.Collections.Generic

    /// getEnumerator returns an enumerator for the given sequence.
    let inline getEnumerator (source: 'a seq) = source.GetEnumerator()
    /// moveNext returns true if the given enumerator has a next element.
    let inline moveNext (enumerator: IEnumerator<'a>) = enumerator.MoveNext()
    /// getCurrent returns the current element of the given enumerator.
    let inline getCurrent (enumerator: IEnumerator<'a>) = enumerator.Current

    /// tryMaxWithCompareBy returns the maximum element of the given sequence using the given compare function and key selector.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same maximum key, the first element is returned.
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

    /// tryMaxWithCompare returns the maximum element of the given sequence using the given compare function.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same maximum key, the first element is returned.
    let inline tryMaxWithCompare compare = tryMaxWithCompareBy compare id

    /// tryMaxBy returns the maximum element of the given sequence using the given key selector.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same maximum key, the first element is returned.
    let inline tryMaxBy keySelector =
        tryMaxWithCompareBy Comparer.defaultOf keySelector

    /// tryMax returns the maximum element of the given sequence.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same maximum key, the first element is returned.
    let inline tryMax source =
        tryMaxWithCompare Comparer.defaultOf source

    /// tryMinWithCompareBy returns the minimum element of the given sequence using the given compare function and key selector.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same minimum key, the first element is returned.
    let inline tryMinWithCompareBy compare keySelector =
        tryMaxWithCompareBy (Comparer.inverse compare) keySelector

    /// tryMinWithCompare returns the minimum element of the given sequence using the given compare function.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same minimum key, the first element is returned.
    let inline tryMinWithCompare compare = tryMinWithCompareBy compare id

    /// tryMinBy returns the minimum element of the given sequence using the given key selector.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same minimum key, the first element is returned.
    let inline tryMinBy keySelector =
        tryMinWithCompareBy Comparer.defaultOf keySelector

    /// tryMin returns the minimum element of the given sequence.
    /// If the sequence is empty, None is returned.
    /// If the sequence contains multiple elements with the same minimum key, the first element is returned.
    let inline tryMin source =
        tryMinWithCompare Comparer.defaultOf source

    /// scanCond scans the given sequence using the given function and initial accumulator.
    /// The function is called with the current accumulator and the current element.
    /// The function can return Continue, ContinueWith, Break or BreakWith to control the behavior of the scan.
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

    /// foldCond folds the given sequence using the given function and initial accumulator.
    /// The function is called with the current accumulator and the current element.
    /// The function can return Continue, ContinueWith, Break or BreakWith to control the behavior of the fold.
    /// The final accumulator is returned.
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
