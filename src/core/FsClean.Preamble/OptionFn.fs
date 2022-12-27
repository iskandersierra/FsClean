namespace FsClean

open System

type OptionFn<'a, 'b> = 'a -> 'b option

[<RequireQualifiedAccess>]
module OptionFn =
    let pipeTo (second: OptionFn<'b, 'c>) (first: OptionFn<'a, 'b>) : OptionFn<'a, 'c> = first >> Option.bind second

    let ofMap map : OptionFn<'a, 'b> = fun a -> Map.tryFind a map

    let ofDict dict : OptionFn<'a, 'b> = fun a -> Dict.tryFind a dict

type DualOptionFn<'a, 'b> =
    { forward: OptionFn<'a, 'b>
      backward: OptionFn<'b, 'a> }

[<RequireQualifiedAccess>]
module DualOptionFn =
    let reverse converter : DualOptionFn<'b, 'a> =
        { forward = converter.backward
          backward = converter.forward }

    let pipeTo (second: DualOptionFn<'b, 'c>) (first: DualOptionFn<'a, 'b>) : DualOptionFn<'a, 'c> =
        { forward = first.forward |> OptionFn.pipeTo second.forward
          backward = second.backward |> OptionFn.pipeTo first.backward }

    let ofMaps forwardMap backwardMap : DualOptionFn<'a, 'b> =
        { forward = OptionFn.ofMap forwardMap
          backward = OptionFn.ofMap backwardMap }

    let ofDict forwardDict backwardDict : DualOptionFn<'a, 'b> =
        { forward = OptionFn.ofDict forwardDict
          backward = OptionFn.ofDict backwardDict }

    let ofPairs pairs : DualOptionFn<'a, 'b> =
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

    let ofPairsDictWithComparers sourceComparer targetComparer pairs : DualOptionFn<'a, 'b> =
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
        (typeToSchema: DualOptionFn<'t, 's>)
        (converter: DualOptionFn<'a, 't * 'b>)
        : DualOptionFn<'a, 's * 'b> =
        let forward a =
            //let aType = getSourceType source
            match converter.forward a with
            | None -> failwithf "Could not convert source %A" a
            | Some (t, b) ->
                match typeToSchema.forward t with
                | None -> failwithf "No schema found for type %A" t
                | Some s -> Some(s, b)

        let backward (s, b) =
            match typeToSchema.backward s with
            | None -> failwithf "No type found for schema %A" s
            | Some t ->
                match converter.backward (t, b) with
                | None -> failwithf "Could not de-convert %A for type %A" b t
                | Some a -> Some a

        { forward = forward
          backward = backward }

    [<RequireQualifiedAccess>]
    module String =
        open System.Text

        let ofEncoding (encoding: Encoding) =
            let forward source =
                let aTarget = encoding.GetBytes(source: string)
                Some aTarget

            let backward aTarget =
                let aSource = encoding.GetString(aTarget: byte [])
                Some aSource

            { forward = forward
              backward = backward }

        let utf8 = ofEncoding Encoding.UTF8

    [<RequireQualifiedAccess>]
    module Json =
        open System.Text.Json

        let serializer: DualOptionFn<obj, Type * string> =
            let forward source =
                let aType = source.GetType()

                let aTarget = JsonSerializer.Serialize(source: obj)

                Some(aType, aTarget)

            let backward (aType, aTarget) =
                JsonSerializer.Deserialize((aTarget: string), (aType: Type))
                |> Some

            { forward = forward
              backward = backward }

        let binarySerializer: DualOptionFn<obj, Type * byte []> =
            let prepend t x = t, x
            let prepend t = prepend t |> Option.map

            serializer
            |> pipeTo
                { forward = fun (t, s) -> String.utf8.forward s |> prepend t
                  backward = fun (t, b) -> String.utf8.backward b |> prepend t }
