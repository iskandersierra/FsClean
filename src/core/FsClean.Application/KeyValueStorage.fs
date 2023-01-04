namespace FsClean.Application.KeyValueStorage

open System.Threading
open System.Threading.Tasks

open FsClean

type SaveKeyValue<'key, 'value> = CancellationToken -> 'key -> 'value -> Task

type RemoveKeyValue<'key> = CancellationToken -> 'key -> Task

type TryLoadKeyValue<'key, 'value> = CancellationToken -> 'key -> Task<'value option>

type TryLoadManyKeyValue<'key, 'value> = CancellationToken -> 'key seq -> Task<('key * 'value) array>

type TryLoadFirstKeyValue<'key, 'value> = CancellationToken -> 'key seq -> Task<('key * 'value) option>

module KeyValueStore =
    open FsToolkit.ErrorHandling
    
    let tryLoadMany (tryLoad: TryLoadKeyValue<'key, 'value>) ct keys =
        task {
            let result = ResizeArray()

            for key in keys do
                let! value = tryLoad ct key

                match value with
                | Some value -> result.Add(key, value)
                | None -> ()

            return result.ToArray()
        }

    let tryLoadFirst (tryLoad: TryLoadKeyValue<'key, 'value>) ct keys =
        let rec loop e =
            task {
                match Seq.moveNext e with
                | true ->
                    match! tryLoad ct (Seq.getCurrent e) with
                    | Some value -> return Some(Seq.getCurrent e, value)
                    | None -> return! loop e
                | false -> return None
            }

        task {
            use e = Seq.getEnumerator keys
            return! loop e
        }

    let mapSave toInnerKey toInnerValue (save: SaveKeyValue<'innerKey, 'innerValue>) : SaveKeyValue<'outterKey, 'outterValue> =
        fun ct key value -> save ct (toInnerKey key) (toInnerValue value)

    let mapRemove toInnerKey (remove: RemoveKeyValue<'innerKey>) : RemoveKeyValue<'outterKey> =
        fun ct key -> remove ct (toInnerKey key)

    let mapTryLoad toInnerKey toOutterValue (tryLoad: TryLoadKeyValue<'innerKey, 'innerValue>) : TryLoadKeyValue<'outterKey, 'outterValue> =
        fun ct key ->
            taskOption {
                let! value = tryLoad ct (toInnerKey key)
                return toOutterValue value
            }

    let mapTryLoadMany (keyFn: DualFn<_, _>) toOutterValue (tryLoadMany: TryLoadManyKeyValue<'innerKey, 'innerValue>) : TryLoadManyKeyValue<'outterKey, 'outterValue> =
        fun ct keys ->
            task {
                let innerKeys = keys |> Seq.map keyFn.forward
                let! result = tryLoadMany ct innerKeys
                let result = result |> Array.map (fun (k, v) -> keyFn.backward k, toOutterValue v)
                return result
            }

    let mapTryLoadFirst (keyFn: DualFn<_, _>) toOutterValue (tryLoadFirst: TryLoadFirstKeyValue<'innerKey, 'innerValue>) : TryLoadFirstKeyValue<'outterKey, 'outterValue> =
        fun ct keys ->
            taskOption {
                let innerKeys = keys |> Seq.map keyFn.forward
                let! innerKey, innerValue = tryLoadFirst ct innerKeys
                return keyFn.backward innerKey, toOutterValue innerValue
            }
