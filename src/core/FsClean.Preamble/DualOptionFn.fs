namespace FsClean

/// type DualOptionFn defines an optional function that can be used in both directions.
/// In each direction, the function returns None if the input value is not valid.
type DualOptionFn<'a, 'b> =
    { forward: OptionFn<'a, 'b>
      backward: OptionFn<'b, 'a> }

/// module DualOptionFn defines functions for working with dual optional functions.
[<RequireQualifiedAccess>]
module DualOptionFn =
    /// create creates a dual optional function from the given forward and backward functions.
    let inline create forward backward : DualOptionFn<'a, 'b> =
        { forward = forward
          backward = backward }

    /// id is a dual optional function that returns its argument in both directions.
    let id : DualOptionFn<'a, 'a> =
        { forward = OptionFn.id
          backward = OptionFn.id }

    /// reverse returns a dual optional function that is the reverse of the given dual optional function.
    let reverse converter : DualOptionFn<'b, 'a> =
        create converter.backward converter.forward

    /// pipeTo returns a dual optional function that applies the given dual optional functions in sequence.
    /// In the forward direction, the first forward function is applied, then the second forward function.
    /// In the backward direction, the second backward function is applied, then the first backward function.
    let pipeTo (second: DualOptionFn<'b, 'c>) (first: DualOptionFn<'a, 'b>) : DualOptionFn<'a, 'c> =
        create (first.forward |> OptionFn.pipeTo second.forward) (second.backward |> OptionFn.pipeTo first.backward)

    /// ofMap creates a dual optional function from the given maps.
    let ofMaps forwardMap backwardMap : DualOptionFn<'a, 'b> =
        create (OptionFn.ofMap forwardMap) (OptionFn.ofMap backwardMap)

    /// ofDict creates a dual optional function from the given dictionaries.
    let ofDict forwardDict backwardDict : DualOptionFn<'a, 'b> =
        create (OptionFn.ofDict forwardDict) (OptionFn.ofDict backwardDict)

    /// ofPairs creates a dual optional function from the given pairs.
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

    /// ofPairsDictWithComparers creates a dual optional function from the given pairs.
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

    /// ofPairsDict creates a dual optional function from the given pairs.
    let ofPairsDict pairs =
        ofPairsDictWithComparers (EqualityComparer.defaultOf<_>) (EqualityComparer.defaultOf<_>) pairs

    /// withSchema returns a dual optional function that converts a source value to a target value and a schema.
    let withSchema
        (typeToSchema: DualOptionFn<'t, 's>)
        (converter: DualOptionFn<'a, 't * 'b>)
        : DualOptionFn<'a, 's * 'b> =
        create
            (fun a ->
                match converter.forward a with
                | None -> failwithf "Could not convert source %A" a
                | Some (t, b) ->
                    match typeToSchema.forward t with
                    | None -> failwithf "No schema found for type %A" t
                    | Some s -> Some(s, b)
            )
            (fun (s, b) ->
                match typeToSchema.backward s with
                | None -> failwithf "No type found for schema %A" s
                | Some t ->
                    match converter.backward (t, b) with
                    | None -> failwithf "Could not de-convert %A for type %A" b t
                    | Some a -> Some a
            )

    /// module String defines functions for working with dual optional functions that convert to and from strings.
    [<RequireQualifiedAccess>]
    module String =
        /// ofEncoding returns a dual optional function that converts to and from strings using the given encoding.
        let ofEncoding encoding =
            create (OptionFn.String.toEncodingBytes encoding) (OptionFn.String.ofEncodingBytes encoding)

        /// utf8 is a dual optional function that converts to and from strings using UTF8 encoding.
        let utf8 = create OptionFn.String.toUtf8 OptionFn.String.ofUtf8

    /// module Json defines functions for working with dual optional functions that convert to and from JSON.
    [<RequireQualifiedAccess>]
    module Json =
        open System
        open System.Text.Json

        /// serializer is a dual optional function that converts to and from JSON.
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

        /// binarySerializer is a dual optional function that converts to and from JSON.
        let binarySerializer: DualOptionFn<obj, Type * byte []> =
            let prepend t x = t, x
            let prepend t = prepend t |> Option.map

            serializer
            |> pipeTo
                { forward = fun (t, s) -> String.utf8.forward s |> prepend t
                  backward = fun (t, b) -> String.utf8.backward b |> prepend t }
