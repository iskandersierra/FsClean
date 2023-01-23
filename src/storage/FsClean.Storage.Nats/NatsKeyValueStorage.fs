module FsClean.Storage.Nats.NatsKeyValueStorage

//open System.Threading
//open System.Threading.Tasks

//open FsClean
//open NATS.Client
//open NATS.Client.JetStream
//open NATS.Client.KeyValue
//open FsToolkit.ErrorHandling

//type NatsKeyValueStore =
//    { createBytes: CancellationToken -> string -> byte [] -> Task<uint64>

//      delete: CancellationToken -> string -> Task

//      get: CancellationToken -> string -> TaskOption<KeyValueEntry>
//      getRevision: CancellationToken -> string -> uint64 -> TaskOption<KeyValueEntry>

//      putBytes: CancellationToken -> string -> byte [] -> Task<uint64>
//      putLong: CancellationToken -> string -> int64 -> Task<uint64>
//      putString: CancellationToken -> string -> string -> Task<uint64>

//      tryLoad: CancellationToken -> string -> TaskOption<byte []> }

//type Options =
//    { conn: IConnection
//      bucketName: string
//      keyValueOptions: KeyValueOptions }

//let defaultOptions =
//    { conn = null
//      bucketName = "key_values"
//      keyValueOptions = KeyValueOptions.Builder().Build() }

//let initializeWithConfig config options =
//    let management =
//        options.conn.CreateKeyValueManagementContext(options.keyValueOptions)

//    let config =
//        KeyValueConfiguration
//            .Builder(config)
//            .WithName(options.bucketName)
//            .Build()

//    management.Create(config)

//let initialize =
//    KeyValueConfiguration.Builder().Build()
//    |> initializeWithConfig

//let create options =
//    let bucket =
//        options.conn.CreateKeyValueContext(options.bucketName, options.keyValueOptions)

//    { createBytes = fun ct key (value: byte []) -> task { return bucket.Create(key, value) }

//      delete = fun ct key -> task { bucket.Delete(key) }

//      get =
//          fun ct key ->
//              task {
//                  try
//                      return Some(bucket.Get(key))
//                  with
//                  | :? NATSJetStreamException as exn when exn.ErrorCode = 404 -> return None
//              }

//      getRevision =
//          fun ct key revision ->
//              task {
//                  try
//                      return Some(bucket.Get(key, revision))
//                  with
//                  | :? NATSJetStreamException as exn when exn.ErrorCode = 404 -> return None
//              }

//      putBytes = fun ct key (value: byte []) -> task { return bucket.Put(key, value) }
//      putLong = fun ct key (value: int64) -> task { return bucket.Put(key, value) }
//      putString = fun ct key (value: string) -> task { return bucket.Put(key, value) }

//      tryLoad =
//          fun ct key ->
//              task {
//                  try
//                      let entry = bucket.Get(key)
//                      return Some entry.Value
//                  with
//                  | :? NATSJetStreamException as exn when exn.ErrorCode = 404 -> return None
//              } }
