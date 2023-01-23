/// module Json defines functions and types for working with JSON.
[<RequireQualifiedAccess>]
module FsClean.Json

open System
open System.Text.Json

open FsClean

/// serializeTypedWithOptions serializes the given value to JSON, using the given options, and returns the type of the value and the JSON.
let serializeTypedWithOptions (options: JsonSerializerOptions) (value: obj) =
    let aType = getType value
    let json = JsonSerializer.Serialize(value, options)
    aType, json

/// deserializeTypedWithOptions deserializes the given JSON string to an object of the given type, using the given options, and returns the object.
let deserializeTypedWithOptions (options: JsonSerializerOptions) (aType, json) =
    JsonSerializer.Deserialize((json: string), (aType: Type), options)

/// serializeTyped serializes the given value to JSON and returns the type of the value and the JSON.
let serializeTyped (value: obj) =
    serializeTypedWithOptions JsonSerializerOptions.Default value

/// deserializeTyped deserializes the given JSON string to an object of the given type and returns the object.
let deserializeTyped (aType, json) =
    deserializeTypedWithOptions JsonSerializerOptions.Default (aType, json)


/// serializeWithOptions serializes the given value to JSON using the given options and returns the JSON.
let serializeWithOptions<'value> (options: JsonSerializerOptions) (value: 'value) =
    JsonSerializer.Serialize<'value>(value, options)

/// deserializeWithOptions deserializes the given JSON string to an object of the given type using the given options and returns the object.
let deserializeWithOptions<'value> (options: JsonSerializerOptions) (json: string) =
    JsonSerializer.Deserialize<'value>(json, options)

let serialize<'value> (value: 'value) = serializeWithOptions<'value> JsonSerializerOptions.Default value

let deserialize<'value> json = deserializeWithOptions<'value> JsonSerializerOptions.Default json
