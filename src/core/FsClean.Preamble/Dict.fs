[<RequireQualifiedAccess>]
module FsClean.Dict

open System.Collections.Generic

open FsClean

let emptyWithComparer comparer = Dictionary(comparer = comparer)

let empty () = emptyWithComparer (EqualityComparer.defaultOf<_>)

let add key value (dict: #IDictionary<_, _>) = dict.Add(key, value); dict

let remove key (dict: #IDictionary<_, _>) = dict.Remove(key = key) |> ignore; dict

let containsKey key (dict: #IDictionary<_, _>) = dict.ContainsKey(key)

let tryFind key (source: #IDictionary<_, _>) =
    match source.TryGetValue(key) with
    | true, value -> Some value
    | false, _ -> None
