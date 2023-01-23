namespace FsClean

/// type OptionFn defines a function that returns an option.
type OptionFn<'a, 'b> = 'a -> 'b option

/// module OptionFn defines functions and types for working with option functions.
[<RequireQualifiedAccess>]
module OptionFn =
    /// id return a function that given a value returns Some of that value.
    let id<'a> : OptionFn<'a, 'a> = fun a -> Some a

    /// konst return a function that given a value returns the given value.
    let konst k : OptionFn<'a, 'b> = fun _ -> Some k

    /// pipeTo returns a coupled function that applies the given functions in sequence.
    let pipeTo (second: OptionFn<'b, 'c>) (first: OptionFn<'a, 'b>) : OptionFn<'a, 'c> = first >> Option.bind second

    /// ofMap creates an option function from the given map.
    /// If the key is not found in the map, the function will return None.
    /// If the key is found in the map, the function will return Some of the value.
    let ofMap map : OptionFn<'a, 'b> = fun a -> Map.tryFind a map

    /// ofDict creates an option function from the given dictionary.
    /// If the key is not found in the dictionary, the function will return None.
    /// If the key is found in the dictionary, the function will return Some of the value.
    let ofDict dict : OptionFn<'a, 'b> = fun a -> Dict.tryFind a dict

    /// tryWith returns a function that applies the given function and returns Some of the result or None if the function throws an exception.
    let tryWith fn : OptionFn<_, _> =
        fun a ->
            try
                fn a |> Some
            with
            | _ -> None


    /// module String defines functions for working with string conversions.
    [<RequireQualifiedAccess>]
    module String =
        /// toEncodingBytes returns an option function that converts a string to a byte array using the given encoding.
        let toEncodingBytes encoding = tryWith (String.toEncodingBytes encoding)
        /// ofEncodingBytes returns an option function that converts a byte array to a string using the given encoding.
        let ofEncodingBytes encoding = tryWith (String.ofEncodingBytes encoding)

        /// toUtf8 returns an option function that converts a string to a byte array using UTF8 encoding.
        let toUtf8 = tryWith String.toUtf8
        /// ofUtf8 returns an option function that converts a byte array to a string using UTF8 encoding.
        let ofUtf8 = tryWith String.ofUtf8

    /// module Json defines functions for working with JSON conversions.
    [<RequireQualifiedAccess>]
    module Json =
        open System

        /// serializeTypedWithOptions returns an option function that serializes the given object to JSON using the given options.
        let serializeTypedWithOptions options = tryWith (Json.serializeTypedWithOptions options)
        /// deserializeTypedWithOptions returns an option function that deserializes the given JSON string to an object of the given type using the given options.
        let deserializeTypedWithOptions options = tryWith (Json.deserializeTypedWithOptions options)

        /// serializeTyped returns an option function that serializes the given object to JSON.
        let serializeTyped : OptionFn<obj, (Type * string)> = fun a -> tryWith Json.serializeTyped a
        /// deserializeTyped returns an option function that deserializes the given JSON string to an object of the given type.
        let deserializeTyped : OptionFn<Type * string, obj> = fun s -> tryWith Json.deserializeTyped s

        /// serializeWithOptions returns an option function that serializes the given object to JSON using the given options.
        let serializeWithOptions options = tryWith (Json.serializeWithOptions options)
        /// deserializeWithOptions returns an option function that deserializes the given JSON string to an object of the given type using the given options.
        let deserializeWithOptions options = tryWith (Json.deserializeWithOptions options)

        /// serialize returns an option function that serializes the given object to JSON.
        let serialize : OptionFn<'a, string> = fun a -> tryWith Json.serialize a
        /// deserialize returns an option function that deserializes the given JSON string to an object of the given type.
        let deserialize : OptionFn<string, 'a> = fun s -> tryWith Json.deserialize s
