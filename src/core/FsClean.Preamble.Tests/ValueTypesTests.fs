module FsClean.ValueTypesTests

open System
open Xunit
open Swensen.Unquote
open FsCheck
open FsCheck.Xunit
open Validus

open FsClean

let isValidLimitedString fn minLength maxLength field value result =
    let expected = String.trimOrNull value

    let expected =
        if String.IsNullOrEmpty expected then
            Error(ValidationErrors.create field [ $"'{field}' must not be empty" ])
        elif expected.Length < minLength
             || expected.Length > maxLength then
            Error(ValidationErrors.create field [ $"'{field}' must have between {minLength} and {maxLength} characters" ])
        else
            Ok(fn expected)

    test <@ result = expected @>

let isValidOptionalLimitedString fn minLength maxLength field value result =
    let value = String.trimOrNull value

    let expected =
        if String.isNullOrEmpty value then
            Ok None
        elif String.length value < minLength
             || String.length value > maxLength then
            Error(ValidationErrors.create field [ $"'{field}' must have between {minLength} and {maxLength} characters" ])
        else
            Ok(fn (Some value))

    test <@ result = expected @>

let isValidEntityId fn =
    isValidLimitedString fn EntityId.MinLength EntityId.MaxLength

let isValidOptionalEntityId fn =
    isValidOptionalLimitedString fn EntityId.MinLength EntityId.MaxLength


[<Property>]
let ``LimitedString.validate MUST validate identifiers`` (PositiveInt minLength) (PositiveInt length) () field value =
    let maxLength = minLength + length

    LimitedString.create minLength maxLength field value
    |> isValidLimitedString id minLength maxLength field value

[<Property>]
let ``EntityId.validate MUST validate identifiers`` field value =
    EntityId.create field value
    |> isValidEntityId id field value
