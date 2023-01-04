[<RequireQualifiedAccess>]
module FsClean.Json

open System
open System.Text.Json

open FsClean

let serializeTypedWithOptions (options: JsonSerializerOptions) (value: obj) =
    let aType = getType value
    let aTarget = JsonSerializer.Serialize(value, options)
    aType, aTarget

let deserializeTypedWithOptions (options: JsonSerializerOptions) (aType, aTarget) =
    JsonSerializer.Deserialize((aTarget: string), (aType: Type), options)

let serializeTyped : obj -> _ =
    serializeTypedWithOptions JsonSerializerOptions.Default

let deserializeTyped : (Type * string) -> obj =
    deserializeTypedWithOptions JsonSerializerOptions.Default
