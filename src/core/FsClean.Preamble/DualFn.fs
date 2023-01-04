namespace FsClean

type DualFn<'a, 'b> =
    { forward: Fn<'a, 'b>
      backward: Fn<'b, 'a> }

[<RequireQualifiedAccess>]
module DualFn =
    let id : DualFn<'a, 'a> =
        { forward = id
          backward = id }

    let reverse converter : DualFn<'b, 'a> =
        { forward = converter.backward
          backward = converter.forward }

    let pipeTo (second: DualFn<'b, 'c>) (first: DualFn<'a, 'b>) : DualFn<'a, 'c> =
        { forward = first.forward |> Fn.pipeTo second.forward
          backward = second.backward |> Fn.pipeTo first.backward }
      
    let ofMaps forwardMap backwardMap : DualFn<'a, 'b> =
        { forward = Fn.ofMap forwardMap
          backward = Fn.ofMap backwardMap }
      
    let ofDict forwardDict backwardDict : DualFn<'a, 'b> =
        { forward = Fn.ofDict forwardDict
          backward = Fn.ofDict backwardDict }

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

    let ofPairsDict pairs =
        ofPairsDictWithComparers (EqualityComparer.defaultOf<_>) (EqualityComparer.defaultOf<_>) pairs

    let withSchema
        (typeToSchema: DualFn<'t, 's>)
        (converter: DualFn<'a, 't * 'b>)
        : DualFn<'a, 's * 'b> =
        { forward = fun a ->
            let (t, b) = converter.forward a
            let s = typeToSchema.forward t
            (s, b)
          backward = fun (s, b) ->
            let t = typeToSchema.backward s
            let a = converter.backward (t, b)
            a }

    [<RequireQualifiedAccess>]
    module String =
        let ofEncoding encoding =
            { forward = String.toEncodingBytes encoding
              backward = String.ofEncodingBytes encoding }

        let utf8 =
            { forward = String.toUtf8
              backward = String.ofUtf8 }

    [<RequireQualifiedAccess>]
    module Json =
        let typedWithOptions options =
            { forward = Json.serializeTypedWithOptions options
              backward = Json.deserializeTypedWithOptions options }

        let typed =
            { forward = Json.serializeTyped
              backward = Json.deserializeTyped }

        let binaryTypedWithOptions options =
            { forward =
                Json.serializeTypedWithOptions options
                >> fun (t, s) -> t, String.toUtf8 s
              backward =
                fun (t, s) -> t, String.ofUtf8 s
                >> Json.deserializeTypedWithOptions options }
