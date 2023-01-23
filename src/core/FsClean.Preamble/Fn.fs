namespace FsClean

/// type Fn defines a function that takes a value of type 'a and returns a value of type 'b.
type Fn<'a, 'b> = 'a -> 'b

/// module Fn defines functions for working with functions.
[<RequireQualifiedAccess>]
module Fn =
    /// id is a unary function that returns its argument.
    let id<'a> : Fn<'a, 'a> = id

    /// konst is a binary function that returns its first argument.
    let konst k : Fn<'a, 'b> = konst k

    /// pipeTo returns a coupled function that applies the given functions in sequence.
    let pipeTo (second: Fn<'b, 'c>) (first: Fn<'a, 'b>) : Fn<'a, 'c> = first >> second

    /// ofMap creates a function from the given map.
    /// If the key is not found in the map, the function will throw an exception.
    let ofMap map : Fn<'a, 'b> = fun key -> Map.find key map

    /// ofDict creates a function from the given dictionary.
    /// If the key is not found in the dictionary, the function will throw an exception.
    let ofDict map : Fn<'a, 'b> = fun key -> Dict.find key map
