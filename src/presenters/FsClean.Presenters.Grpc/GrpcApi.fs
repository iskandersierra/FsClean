namespace FsClean.Presenters.Grpc

open System

open Google.Protobuf.Collections
open Google.Protobuf.FSharp.WellKnownTypes

open FsToolkit.ErrorHandling

open FsClean.Domain

[<RequireQualifiedAccess>]

module GrpcApi =
    let toValidationErrorData (errors: Map<string, string list>) : FsClean.Presenters.Grpc.ValidationErrorData =
        errors
        |> Map.toSeq
        |> Seq.map (fun (field, errors) ->
            { FsClean.Presenters.Grpc.ValidationError.empty () with
                Field = field
                Messages = RepeatedField.ofSeq errors })
        |> RepeatedField.ofSeq
        |> fun errors -> { FsClean.Presenters.Grpc.ValidationErrorData.empty () with Errors = errors }


    let toDomainError (error: DomainError) : FsClean.Presenters.Grpc.DomainError =
        let errorData =
            match error.errorData with
            | Failure -> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.Failure true
            | Unexpected -> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.Unexpected true
            | NotFound -> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.NotFound true
            | Unauthorized -> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.Unauthorized true
            | Validation errors ->
                toValidationErrorData errors
                |> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.Validation
            | Conflict errors ->
                toValidationErrorData errors
                |> FsClean.Presenters.Grpc.DomainError.Types.ErrorData.Conflict

        { FsClean.Presenters.Grpc.DomainError.empty () with
            Code = error.code
            Description = error.description
            Service = error.service |> Option.toObj
            Entity = error.entity |> Option.toObj
            Operation = error.operation |> Option.toObj
            EntityId = error.entityId |> Option.toObj
            ErrorData = ValueOption.Some errorData }
