/// module Dict defines functions and types for working with dictionaries.
[<RequireQualifiedAccess>]
module FsClean.Dict

open System.Collections.Generic

open FsClean

/// emptyWithComparer creates an empty dictionary with the given equality comparer.
let emptyWithComparer comparer = Dictionary(comparer = comparer)

/// empty creates an empty dictionary with the default equality comparer.
let empty () = emptyWithComparer (EqualityComparer.defaultOf<_>)

/// add adds the given key and value to the given dictionary.
/// The dictionary is returned for convenience.
let add key value (dict: #IDictionary<_, _>) = dict.Add(key, value); dict

/// remove removes the given key from the given dictionary.
/// The dictionary is returned for convenience.
let remove key (dict: #IDictionary<_, _>) = dict.Remove(key = key) |> ignore; dict

/// containsKey returns true if the given key is in the given dictionary.
let containsKey key (dict: #IDictionary<_, _>) = dict.ContainsKey(key)

/// tryFind returns Some value if the given key is in the given dictionary, otherwise None.
let tryFind key (source: #IDictionary<_, _>) =
    match source.TryGetValue(key) with
    | true, value -> Some value
    | false, _ -> None

/// find returns the value for the given key if the key is in the given dictionary, otherwise raises a KeyNotFoundException.
let find key (source: #IDictionary<_, _>) =
    match source.TryGetValue(key) with
    | true, value -> value
    | false, _ -> raise (KeyNotFoundException(sprintf "Key %A not found" key))
