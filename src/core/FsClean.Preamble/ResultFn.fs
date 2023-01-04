namespace FsClean

type ResultFn<'a, 'b, 'e> = 'a -> Result<'b, 'e>

[<RequireQualifiedAccess>]
module ResultFn =
    let id<'a, 'e> : ResultFn<'a, 'a, 'e> = Ok

    let konst k : ResultFn<'a, 'b, 'e> = fun _ -> Ok k

    let pipeTo (second: ResultFn<'b, 'c, 'e>) (first: ResultFn<'a, 'b, 'e>) : ResultFn<'a, 'c, 'e> =
        first >> Result.bind second

    let tryWith fn : ResultFn<'a, 'b, exn> =
        fun a ->
            try
                fn a |> Ok
            with
            | exn -> Error exn

    let ofMap map = map |> flip Map.find |> tryWith

    let ofDict dict = dict |> flip Dict.find |> tryWith


    [<RequireQualifiedAccess>]
    module String =
        let toEncodingBytes encoding =
            tryWith (String.toEncodingBytes encoding)

        let ofEncodingBytes encoding =
            tryWith (String.ofEncodingBytes encoding)

        let toUtf8 = tryWith String.toUtf8
        let ofUtf8 = tryWith String.ofUtf8


    [<RequireQualifiedAccess>]
    module Json =
        open System

        let serializeTypedWithOptions options =
            tryWith (Json.serializeTypedWithOptions options)

        let deserializeTypedWithOptions options =
            tryWith (Json.deserializeTypedWithOptions options)

        let serializeTyped: ResultFn<obj, _, _> = tryWith Json.serializeTyped
        let deserializeTyped: ResultFn<Type * string, _, _> = tryWith Json.deserializeTyped
