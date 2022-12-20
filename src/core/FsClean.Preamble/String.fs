[<RequireQualifiedAccess>]
module FsClean.String

open System

let isNullOrEmpty (value: string) = String.IsNullOrEmpty value
let isNullOrWhiteSpace (value: string) = String.IsNullOrWhiteSpace value

let trim (str: string) = str.Trim()
let trimStart (str: string) = str.TrimStart()
let trimEnd (str: string) = str.TrimEnd()

let trimChar ch (str: string) = str.Trim(ch: char)
let trimStartChar ch (str: string) = str.TrimStart(ch: char)
let trimEndChar ch (str: string) = str.TrimEnd(ch: char)

let trimChars ch (str: string) = str.Trim(ch: char [])
let trimStartChars ch (str: string) = str.TrimStart(ch: char [])
let trimEndChars ch (str: string) = str.TrimEnd(ch: char [])

let trimOrNull (str: string) = match str with null -> null | _ -> trim str
let trimStartOrNull (str: string) = match str with null -> null | _ -> trimStart str
let trimEndOrNull (str: string) = match str with null -> null | _ -> trimEnd str

let trimOrEmpty (str: string) = match str with null -> "" | _ -> trim str
let trimStartOrEmpty (str: string) = match str with null -> "" | _ -> trimStart str
let trimEndOrEmpty (str: string) = match str with null -> "" | _ -> trimEnd str
