namespace FsClean.Application.KeyValueStorage

open FsClean

type IKVStore<'key, 'value> =
    abstract tryGet : 'key -> Async<'value voption>
    abstract set : 'key -> 'value -> Async<unit>
    abstract remove : 'key -> Async<unit>

module KVStore =
    let converted
        (toInnerKey: Fn<'key, 'innerKey>)
        (convertValue: DualFn<'value, 'innerValue>)
        (store: IKVStore<'innerKey, 'innerValue>)
        =
        { new IKVStore<'key, 'value> with
            member _.tryGet key =
                async {
                    let innerKey = toInnerKey key
                    let! innerValue = store.tryGet innerKey
                    return innerValue |> ValueOption.map convertValue.backward
                }

            member _.set key value =
                async {
                    let innerKey = toInnerKey key
                    let innerValue = convertValue.forward value
                    do! store.set innerKey innerValue
                }

            member _.remove key =
                async {
                    let innerKey = toInnerKey key
                    do! store.remove innerKey
                } }

    let versionedKey
        (toVersioned: 'key -> 'version -> 'innerKey)
        (versions: ('version * DualFn<'value, 'innerValue>) seq)
        (store: IKVStore<'innerKey, 'innerValue>)
        =
        let versions = Seq.toArray versions
        if Array.length versions = 0 then
            invalidArg "versions" "You must give at least one version"
        else
            { new IKVStore<'key, 'value> with
                member _.tryGet key =
                    let rec loop i =
                        async {
                            if i >= versions.Length then
                                return ValueNone
                            else
                                match! store.tryGet (toVersioned key (fst versions.[i])) with
                                | ValueSome innerValue ->
                                    let value = (snd versions.[i]).backward innerValue
                                    return ValueSome value
                                | ValueNone -> return! loop (i + 1)
                        }
                    loop 0

                member _.set key value =
                    async {
                        let innerKey = toVersioned key (fst versions.[0])
                        let innerValue = (snd versions.[0]).forward value
                        do! store.set innerKey innerValue
                    }

                member _.remove key =
                    async {
                        for (ver, _) in versions do
                            let innerKey = toVersioned key ver
                            do! store.remove innerKey
                    } }

    let versionedStringKey versions = versionedKey (fun k v -> $"%s{k}%s{v}") versions
