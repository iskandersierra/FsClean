namespace FsClean.Application.KeyValueStorage

open System.Threading
open System.Threading.Tasks

type SaveKeyValue<'key, 'value> = CancellationToken -> 'key -> 'value -> Task

type RemoveKeyValue<'key> = CancellationToken -> 'key -> Task

type TryLoadKeyValue<'key, 'value> = CancellationToken -> 'key -> Task<'value option>

type TryLoadManyKeyValue<'key, 'value> = CancellationToken -> 'key seq -> Task<('key * 'value) array>

type TryLoadFirstKeyValue<'key, 'value> = CancellationToken -> 'key seq -> Task<('key * 'value) option>

type KeyValueStore<'key, 'value> =
    { save: SaveKeyValue<'key, 'value>
      remove: RemoveKeyValue<'key>
      tryLoad: TryLoadKeyValue<'key, 'value>
      tryLoadMany: TryLoadManyKeyValue<'key, 'value>
      tryLoadFirst: TryLoadFirstKeyValue<'key, 'value> }

type IKeyValueStorage<'key, 'value> =
    abstract SaveAsync : key: 'key * value: 'value * cancellationToken: CancellationToken -> Task
    abstract RemoveAsync : key: 'key * cancellationToken: CancellationToken -> Task
    abstract TryLoadKeyAsync : key: 'key * cancellationToken: CancellationToken -> Task<'value option>
    abstract TryLoadManyKeyValue : keys: 'key seq * cancellationToken: CancellationToken -> Task<('key * 'value) array>

    abstract TryLoadFirstKeyValue :
        keys: 'key seq * cancellationToken: CancellationToken -> Task<('key * 'value) option>

module KeyValueStore =
    let toInterface (store: KeyValueStore<'key, 'value>) =
        { new IKeyValueStorage<'key, 'value> with
            member __.SaveAsync(key, value, cancellationToken) = store.save cancellationToken key value
            member __.RemoveAsync(key, cancellationToken) = store.remove cancellationToken key
            member __.TryLoadKeyAsync(key, cancellationToken) = store.tryLoad cancellationToken key

            member __.TryLoadManyKeyValue(keys, cancellationToken) =
                store.tryLoadMany cancellationToken keys

            member __.TryLoadFirstKeyValue(keys, cancellationToken) =
                store.tryLoadFirst cancellationToken keys }

    let ofInterface (store: IKeyValueStorage<'key, 'value>) =
        { save = (fun cancellationToken key value -> store.SaveAsync(key, value, cancellationToken))
          remove = (fun cancellationToken key -> store.RemoveAsync(key, cancellationToken))
          tryLoad = (fun cancellationToken key -> store.TryLoadKeyAsync(key, cancellationToken))
          tryLoadMany = (fun cancellationToken keys -> store.TryLoadManyKeyValue(keys, cancellationToken))
          tryLoadFirst = (fun cancellationToken keys -> store.TryLoadFirstKeyValue(keys, cancellationToken)) }
