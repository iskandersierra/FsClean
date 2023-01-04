namespace FsClean

type OptionFn<'a, 'b> = 'a -> 'b option

[<RequireQualifiedAccess>]
module OptionFn =
    let id<'a> : OptionFn<'a, 'a> = Some

    let konst k : OptionFn<'a, 'b> = fun _ -> Some k

    let pipeTo (second: OptionFn<'b, 'c>) (first: OptionFn<'a, 'b>) : OptionFn<'a, 'c> = first >> Option.bind second

    let ofMap map : OptionFn<'a, 'b> = fun a -> Map.tryFind a map

    let ofDict dict : OptionFn<'a, 'b> = fun a -> Dict.tryFind a dict

    let tryWith fn : OptionFn<_, _> =
        fun a ->
            try
                fn a |> Some
            with
            | _ -> None


    [<RequireQualifiedAccess>]
    module String =
        let toEncodingBytes encoding = tryWith (String.toEncodingBytes encoding)
        let ofEncodingBytes encoding = tryWith (String.ofEncodingBytes encoding)

        let toUtf8 = tryWith String.toUtf8
        let ofUtf8 = tryWith String.ofUtf8


    [<RequireQualifiedAccess>]
    module Json =
        open System

        let serializeTypedWithOptions options = tryWith (Json.serializeTypedWithOptions options)
        let deserializeTypedWithOptions options = tryWith (Json.deserializeTypedWithOptions options)

        let serializeTyped : OptionFn<obj, _> = tryWith Json.serializeTyped
        let deserializeTyped : OptionFn<Type * string, _> = tryWith Json.deserializeTyped
