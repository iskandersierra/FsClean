[<RequireQualifiedAccess>]
module FsClean.Dict

open System.Collections.Generic

open FsClean

let emptyWithComparer comparer = Dictionary(comparer = comparer)

let empty () = emptyWithComparer (EqualityComparer.defaultOf<_>)

let add key value (dict: #IDictionary<_, _>) = dict.Add(key, value); dict

let tryFind key (source: #IDictionary<_, _>) =
    match source.TryGetValue(key) with
    | true, value -> Some value
    | false, _ -> None
