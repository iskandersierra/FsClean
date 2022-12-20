namespace FsClean

open Validus

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

    [<Literal>]
    let MinLength = 1

    [<Literal>]
    let MaxLength = 64

    let create field value =
        LimitedString.create MinLength MaxLength field value

    let createOptional field value =
        LimitedString.createOptional MinLength MaxLength field value
