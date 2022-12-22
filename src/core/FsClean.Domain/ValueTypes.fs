namespace FsClean.Domain

open FsClean
open Validus

[<RequireQualifiedAccess>]
module ValueTypes =
    let createOptional create =
        function
        | None -> Ok None
        | Some value -> create value |> Result.map Some

[<RequireQualifiedAccess>]
module LimitedString =
    let private checkLenMessage minLength maxLength =
        let msg =
            $" must have between {minLength} and {maxLength} characters"

        fun field -> $"'{field}'{msg}"

    let private checkLen minLength maxLength =
        Check.WithMessage.String.betweenLen minLength maxLength (checkLenMessage minLength maxLength)

    let create minLength maxLength field value =
        validate {
            let! value = Check.WithMessage.String.notEmpty (sprintf "'%s' must not be empty") field value
            let value = value.Trim()
            let! value = checkLen minLength maxLength field value
            return value
        }

    let createOptional minLength maxLength field value =
        validate {
            if String.isNullOrWhiteSpace value then
                return None
            else
                let value = String.trim value
                let! value = checkLen minLength maxLength field value
                return Some value
        }

[<RequireQualifiedAccess>]
module EntityId =
    open System.Text.RegularExpressions

    [<Literal>]
    let MinLength = 1

    [<Literal>]
    let MaxLength = 64

    let newGuid () = System.Guid.NewGuid().ToString("D")

    let private checkPatternMessage =
        sprintf "'%s' must be a valid identifier"

    let PatternRegex =
        new Regex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private checkPattern =
        Validator.create checkPatternMessage (fun value -> PatternRegex.IsMatch(value: string))

    let create field value =
        validate {
            let! value = LimitedString.create MinLength MaxLength field value
            let! value = checkPattern field value
            return value
        }

    let createOptional field value =
        validate {
            let! value = LimitedString.createOptional MinLength MaxLength field value
            match value with
            | Some value ->
                let! value = checkPattern field value
                return Some value
            | None -> return None
        }
