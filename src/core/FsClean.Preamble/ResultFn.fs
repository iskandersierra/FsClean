namespace FsClean

/// type ResultFn defines a function that takes a value of type 'a and returns a result of type 'b or 'e.
type ResultFn<'a, 'b, 'e> = 'a -> Result<'b, 'e>

/// module ResultFn defines functions for working with functions that return results.
[<RequireQualifiedAccess>]
module ResultFn =
    /// id is a unary function that returns its argument as an Ok result.
    let id<'a, 'e> : ResultFn<'a, 'a, 'e> = Ok

    /// konst is a binary function that returns its first argument as an Ok result.
    let konst k : ResultFn<'a, 'b, 'e> = fun _ -> Ok k

    /// pipeTo returns a coupled function that applies the given functions in sequence.
    let pipeTo (second: ResultFn<'b, 'c, 'e>) (first: ResultFn<'a, 'b, 'e>) : ResultFn<'a, 'c, 'e> =
        first >> Result.bind second

    /// tryWith returns a function that applies the given function and returns its result as an Ok result, or returns the exception as an Error result.
    let tryWith fn : ResultFn<'a, 'b, exn> =
        fun a ->
            try
                fn a |> Ok
            with
            | exn -> Error exn

    /// ofMap creates a function from the given map.
    let ofMap map = map |> flip Map.find |> tryWith

    /// ofDict creates a function from the given dictionary.
    let ofDict dict = dict |> flip Dict.find |> tryWith


    /// module String defines functions for working with strings.
    [<RequireQualifiedAccess>]
    module String =
        /// toEncodingBytes returns a function that converts a string to a byte array using the given encoding.
        let toEncodingBytes encoding =
            tryWith (String.toEncodingBytes encoding)

        /// ofEncodingBytes returns a function that converts a byte array to a string using the given encoding.
        /// If the byte array is not valid for the given encoding, the function will return an error.
        let ofEncodingBytes encoding =
            tryWith (String.ofEncodingBytes encoding)

        /// toUtf8 returns a function that converts a string to a UTF-8 byte array.
        let toUtf8 = tryWith String.toUtf8
        /// ofUtf8 returns a function that converts a UTF-8 byte array to a string.
        /// If the byte array is not valid UTF-8, the function will return an error.
        let ofUtf8 = tryWith String.ofUtf8


    /// module Json defines functions for working with JSON.
    [<RequireQualifiedAccess>]
    module Json =
        open System

        /// serializeTypedWithOptions returns a function that serializes an object to JSON using the given options.
        let serializeTypedWithOptions options =
            tryWith (Json.serializeTypedWithOptions options)

        /// deserializeTypedWithOptions returns a function that deserializes a JSON string to an object of the given type using the given options.
        let deserializeTypedWithOptions options =
            tryWith (Json.deserializeTypedWithOptions options)

        /// serializeTyped returns a function that serializes an object to JSON.
        let serializeTyped: ResultFn<obj, (Type * string), exn> = tryWith Json.serializeTyped
        /// deserializeTyped returns a function that deserializes a JSON string to an object of the given type.
        let deserializeTyped: ResultFn<Type * string, obj, exn> = tryWith Json.deserializeTyped
