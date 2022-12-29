module FsClean.Storage.Nats.NatsKeyValueStorage

open NATS.Client
open NATS.Client.JetStream
open NATS.Client.KeyValue

open FsClean
open FsClean.Application.KeyValueStorage

type Options =
    { conn: IConnection
      bucketName: string
      keyValueOptions: KeyValueOptions }

let defaultOptions =
    { conn = null
      bucketName = "key_values"
      keyValueOptions = KeyValueOptions.Builder().Build() }

let initializeWithConfig config options =
    let management =
        options.conn.CreateKeyValueManagementContext(options.keyValueOptions)

    let config =
        KeyValueConfiguration
            .Builder(config)
            .WithName(options.bucketName)
            .Build()

    management.Create(config)

let initialize =
    KeyValueConfiguration.Builder().Build()
    |> initializeWithConfig

let create options =
    let bucket =
        options.conn.CreateKeyValueContext(options.bucketName, options.keyValueOptions)

    let tryLoadManyInternal keys =
        keys
        |> Seq.collect (fun key ->
            try
                let entry = bucket.Get(key)
                [ key, entry.Value ]
            with
            | :? NATSJetStreamException as exn when exn.ErrorCode = 404 -> [])

    { save = fun ct key (value: byte []) -> task { bucket.Put(key, value) |> ignore }

      remove = fun ct key -> task { bucket.Delete(key) }

      tryLoad =
          fun ct key ->
              task {
                  try
                      let entry = bucket.Get(key)
                      return Some entry.Value
                  with
                  | :? NATSJetStreamException as exn when exn.ErrorCode = 404 -> return None
              }

      tryLoadMany = fun ct keys -> task { return tryLoadManyInternal keys |> Seq.toArray }

      tryLoadFirst = fun ct keys -> task { return tryLoadManyInternal keys |> Seq.tryHead } }
