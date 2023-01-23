namespace FsClean

/// type DualFn defines a function that can be used in both directions.
type DualFn<'a, 'b> =
    { forward: Fn<'a, 'b>
      backward: Fn<'b, 'a> }

/// module DualFn defines functions for working with dual functions.
[<RequireQualifiedAccess>]
module DualFn =
    /// create creates a dual function from the given forward and backward functions.
    let inline create forward backward : DualFn<'a, 'b> =
        { forward = forward
          backward = backward }

    /// id is a dual function that returns its argument in both directions.
    let id: DualFn<'a, 'a> = { forward = id; backward = id }

    /// reverse returns a dual function that is the reverse of the given dual function.
    let reverse converter =
        create converter.backward converter.forward

    /// pipeTo returns a dual function that applies the given dual functions in sequence.
    /// In the forward direction, the first forward function is applied, then the second forward function.
    /// In the backward direction, the second backward function is applied, then the first backward function.
    let pipeTo second first =
        create (first.forward >> second.forward) (second.backward >> first.backward)

    /// ofMap creates a dual function from the given maps.
    let ofMaps forwardMap backwardMap =
        create (Fn.ofMap forwardMap) (Fn.ofMap backwardMap)

    /// ofDict creates a dual function from the given dictionaries.
    let ofDict forwardDict backwardDict =
        create (Fn.ofDict forwardDict) (Fn.ofDict backwardDict)

    /// ofPairs creates a dual function from the given pairs.
    let ofPairs pairs : DualFn<'a, 'b> =
        pairs
        |> Seq.fold
            (fun (forwardMap, backwardMap) (source, target) ->
                match forwardMap |> Map.tryFind source with
                | Some otherTarget -> failwithf "Duplicate source %A with targets %A and %A" source target otherTarget
                | None ->
                    match backwardMap |> Map.tryFind target with
                    | Some otherSource ->
                        failwithf "Duplicate target %A with sources %A and %A" target source otherSource
                    | None -> (forwardMap |> Map.add source target, backwardMap |> Map.add target source))
            (Map.empty, Map.empty)
        |> fun (forwardMap, backwardMap) -> ofMaps forwardMap backwardMap

    /// ofPairsDictWithComparers creates a dual function from the given pairs.
    /// The given comparers are used to compare the source and target values.
    let ofPairsDictWithComparers sourceComparer targetComparer pairs : DualFn<'a, 'b> =
        let sourceToTargetDict = Dict.emptyWithComparer sourceComparer
        let targetToSourceDict = Dict.emptyWithComparer targetComparer

        for (source, target) in pairs do
            match sourceToTargetDict |> Dict.tryFind source with
            | Some otherTarget -> failwithf "Duplicate source %A with targets %A and %A" source target otherTarget
            | None ->
                match targetToSourceDict |> Dict.tryFind target with
                | Some otherSource -> failwithf "Duplicate target %A with sources %A and %A" target source otherSource
                | None ->
                    sourceToTargetDict
                    |> Dict.add source target
                    |> ignore

                    targetToSourceDict
                    |> Dict.add target source
                    |> ignore

        ofDict sourceToTargetDict targetToSourceDict

    /// ofPairsDict creates a dual function from the given pairs.
    let ofPairsDict pairs =
        ofPairsDictWithComparers (EqualityComparer.defaultOf<_>) (EqualityComparer.defaultOf<_>) pairs

    /// withSchema creates a dual function that converts a source value to a target value and a schema.
    let withSchema (typeToSchema: DualFn<'t, 's>) (converter: DualFn<'a, 't * 'b>) : DualFn<'a, 's * 'b> =
        { forward =
            fun a ->
                let (t, b) = converter.forward a
                let s = typeToSchema.forward t
                (s, b)
          backward =
            fun (s, b) ->
                let t = typeToSchema.backward s
                let a = converter.backward (t, b)
                a }

    /// module String defines dual functions for working with strings.
    [<RequireQualifiedAccess>]
    module String =
        /// ofEncoding creates a dual function that converts a string to and from a byte array using the given encoding.
        let ofEncoding encoding =
            { forward = String.toEncodingBytes encoding
              backward = String.ofEncodingBytes encoding }

        /// utf8 is a dual function that converts a string to and from a UTF-8 byte array.
        let utf8 =
            { forward = String.toUtf8
              backward = String.ofUtf8 }

    /// module Json defines dual functions for working with JSON.
    [<RequireQualifiedAccess>]
    module Json =
        /// typedWithOptions creates a dual function that converts a value to and from a JSON string using the given options.
        let typedWithOptions options =
            { forward = Json.serializeTypedWithOptions options
              backward = Json.deserializeTypedWithOptions options }

        /// typed creates a dual function that converts a value to and from a JSON string.
        let typed =
            { forward = Json.serializeTyped
              backward = Json.deserializeTyped }

        /// binaryTypedWithOptions creates a dual function that converts a value to and from a JSON string using the given options.
        let binaryTypedWithOptions options =
            { forward =
                Json.serializeTypedWithOptions options
                >> fun (t, s) -> t, String.toUtf8 s
              backward =
                fun (t, s) -> t, String.ofUtf8 s
                >> Json.deserializeTypedWithOptions options }
