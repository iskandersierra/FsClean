module FsClean.Domain.ValueTypesTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open FsClean
open FsClean.String.Patterns
open Validus


let internal testValidator validator =
    fun value result ->
        let expected = validator value
        test <@ result = expected @>

[<RequireQualifiedAccess>]
module LimitedString =
    let internal testValidateOptional minLength maxLength field =
        let outsideMsg =
            sprintf "'%s' must have between %d and %d characters" field minLength maxLength

        fun value ->
            validate {
                match String.trimOrNull value with
                | IsNullOrEmpty _ -> return None
                | HasLengthOutside minLength maxLength _ ->
                    return! Error(ValidationErrors.create field [ outsideMsg ])
                | value -> return Some value
            }

    let internal testValidate minLength maxLength field =
        let validateOptional =
            testValidateOptional minLength maxLength field

        let emptyMsg = sprintf "'%s' must not be empty" field

        fun value ->
            validate {
                match! validateOptional value with
                | None -> return! Error(ValidationErrors.create field [ emptyMsg ])
                | Some value -> return value
            }

    let testIsValidOptional fn minLength maxLength field =
        testValidator (
            testValidateOptional minLength maxLength field
            >> Result.map (Option.map fn)
        )

    let testIsValid fn minLength maxLength field =
        testValidator (
            testValidate minLength maxLength field
            >> Result.map fn
        )

[<RequireQualifiedAccess>]
module EntityId =
    let internal testValidateOptional field =
        let validateOptionalString =
            LimitedString.testValidateOptional EntityId.MinLength EntityId.MaxLength field

        let validIdMsg =
            sprintf "'%s' must be a valid identifier" field

        fun value ->
            validate {
                match! validateOptionalString value with
                | None -> return None
                | Some (IsNotMatch EntityId.PatternRegex value) ->
                    return! Error(ValidationErrors.create field [ validIdMsg ])
                | Some value -> return Some value
            }

    let internal testValidate field =
        let validateOptional = testValidateOptional field

        let emptyMsg = sprintf "'%s' must not be empty" field

        fun value ->
            validate {
                match! validateOptional value with
                | None -> return! Error(ValidationErrors.create field [ emptyMsg ])
                | Some value -> return value
            }

    let testIsValidOptional fn field =
        testValidator (
            testValidateOptional field
            >> Result.map (Option.map fn)
        )

    let testIsValid fn field =
        testValidator (testValidate field >> Result.map fn)

[<Property>]
let ``LimitedString.create MUST validate identifiers`` (PositiveInt minLength) (PositiveInt length) field value =
    let maxLength = minLength + length

    LimitedString.create minLength maxLength field value
    |> LimitedString.testIsValid id minLength maxLength field value

[<Property>]
let ``LimitedString.createOptional MUST validate identifiers``
    (PositiveInt minLength)
    (PositiveInt length)
    field
    value
    =
    let maxLength = minLength + length

    LimitedString.createOptional minLength maxLength field value
    |> LimitedString.testIsValidOptional id minLength maxLength field value

[<Property>]
let ``EntityId.create MUST validate identifiers`` field value =
    EntityId.create field value
    |> EntityId.testIsValid id field value

[<Property>]
let ``EntityId.createOptional MUST validate identifiers`` field value =
    EntityId.createOptional field value
    |> EntityId.testIsValidOptional id field value
