namespace FsClean.Presenters.Grpc

open System

open Google.Protobuf.Collections
open Google.Protobuf.FSharp.WellKnownTypes

open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module RepeatedField =
    let ofSeq (source: 'a seq) : RepeatedField<'a> =
        let field = RepeatedField<_>()
        field.AddRange(source)
        field

[<RequireQualifiedAccess>]
module Timestamp =
    let fromDateTimeOption (dateTime: DateTime option) : Timestamp voption =
        dateTime
        |> Option.map Timestamp.FromDateTime
        |> ValueOption.ofOption

    let toDateTimeOption (timestamp: Timestamp voption) : DateTime option =
        timestamp
        |> ValueOption.bind (fun d -> d.ToDateTime())
        |> Option.ofValueOption
