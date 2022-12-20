module FsClean.ValueTypesTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open Validus

open FsClean

let internal testValidator validator =
    fun value result ->
        let expected = validator value
        test <@ result = expected @>

[<RequireQualifiedAccess>]
module LimitedString =
    let internal validateOptional minLength maxLength field =
        let outsideMsg =
            sprintf "'%s' must have between %d and %d characters" field minLength maxLength

        fun value ->
            validate {
                match String.trimOrNull value with
                | StringIsNullOrEmpty _ -> return None
                | StringHasLengthOutside minLength maxLength _ ->
                    return! Error(ValidationErrors.create field [ outsideMsg ])
                | value -> return Some value
            }

    let internal validate minLength maxLength field =
        let validateOptional =
            validateOptional minLength maxLength field

        let emptyMsg = sprintf "'%s' must not be empty" field

        fun value ->
            validate {
                match! validateOptional value with
                | None -> return! Error(ValidationErrors.create field [ emptyMsg ])
                | Some value -> return value
            }

    let isValidOptional fn minLength maxLength field =
        testValidator (
            validateOptional minLength maxLength field
            >> Result.map (Option.map fn)
        )

    let isValid fn minLength maxLength field =
        testValidator (
            validate minLength maxLength field
            >> Result.map fn
        )

[<RequireQualifiedAccess>]
module EntityId =
    let internal validateOptional field =
        let validateOptionalString =
            LimitedString.validateOptional EntityId.MinLength EntityId.MaxLength field

        let validIdMsg =
            sprintf "'%s' must be a valid identifier" field

        fun value ->
            validate {
                match! validateOptionalString value with
                | None -> return None
                | Some value ->
                    match value with
                    | StringIsNotMatch EntityId.PatternRegex _ ->
                        return! Error(ValidationErrors.create field [ validIdMsg ])
                    | value -> return Some value
            }

    let internal validate field =
        let validateOptional = validateOptional field

        let emptyMsg = sprintf "'%s' must not be empty" field

        fun value ->
            validate {
                match! validateOptional value with
                | None -> return! Error(ValidationErrors.create field [ emptyMsg ])
                | Some value -> return value
            }

    let isValidOptional fn field =
        testValidator (
            validateOptional field
            >> Result.map (Option.map fn)
        )

    let isValid fn field =
        testValidator (validate field >> Result.map fn)

[<Property>]
let ``LimitedString.create MUST validate identifiers`` (PositiveInt minLength) (PositiveInt length) field value =
    let maxLength = minLength + length

    LimitedString.create minLength maxLength field value
    |> LimitedString.isValid id minLength maxLength field value

[<Property>]
let ``LimitedString.createOptional MUST validate identifiers``
    (PositiveInt minLength)
    (PositiveInt length)
    field
    value
    =
    let maxLength = minLength + length

    LimitedString.createOptional minLength maxLength field value
    |> LimitedString.isValidOptional id minLength maxLength field value

[<Property>]
let ``EntityId.create MUST validate identifiers`` field value =
    EntityId.create field value
    |> EntityId.isValid id field value

[<Property(Replay = "313269454, 297125315")>]
let ``EntityId.createOptional MUST validate identifiers`` field value =
    EntityId.createOptional field value
    |> EntityId.isValidOptional id field value
