[<RequireQualifiedAccess>]
module FsClean.String

open System
open System.Text
open System.Text.RegularExpressions

let isNullOrEmpty (value: string) = String.IsNullOrEmpty value
let isNullOrWhiteSpace (value: string) = String.IsNullOrWhiteSpace value

let isNotNullOrEmpty = isNullOrEmpty >> not
let isNotNullOrWhiteSpace = isNullOrWhiteSpace >> not

let trim (str: string) = str.Trim()
let trimStart (str: string) = str.TrimStart()
let trimEnd (str: string) = str.TrimEnd()

let trimChar ch (str: string) = str.Trim(ch: char)
let trimStartChar ch (str: string) = str.TrimStart(ch: char)
let trimEndChar ch (str: string) = str.TrimEnd(ch: char)

let trimChars ch (str: string) = str.Trim(ch: char [])
let trimStartChars ch (str: string) = str.TrimStart(ch: char [])
let trimEndChars ch (str: string) = str.TrimEnd(ch: char [])

let trimOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trim str

let trimStartOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trimStart str

let trimEndOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trimEnd str

let trimOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trim str

let trimStartOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trimStart str

let trimEndOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trimEnd str

// Encoding

let toEncodingBytes (encoding: Encoding) (str: string) = encoding.GetBytes str
let ofEncodingBytes (encoding: Encoding) (bytes: byte []) = encoding.GetString bytes

let toUtf8 = toEncodingBytes Encoding.UTF8
let ofUtf8 = ofEncodingBytes Encoding.UTF8

module Patterns =
    let private ofCondition predicate =
        fun value ->
            if predicate value then
                Some value
            else
                None

    let (|IsNullOrEmpty|_|) = ofCondition isNullOrEmpty
    let (|IsNullOrWhiteSpace|_|) = ofCondition isNullOrWhiteSpace

    let (|IsNotNullOrEmpty|_|) = ofCondition isNotNullOrEmpty
    let (|IsNotNullOrWhiteSpace|_|) = ofCondition isNotNullOrWhiteSpace

    let (|HasLengthOutside|_|) minLength maxLength =
        ofCondition (fun value ->
            String.length value < minLength
            || String.length value > maxLength)

    let (|HasLengthBetween|_|) minLength maxLength =
        ofCondition (fun value ->
            String.length value >= minLength
            && String.length value <= maxLength)

    let (|HasLength|_|) length value =
        ofCondition (String.length >> (=) length) value

    let (|Match|_|) (regex: Regex) value =
        if isNull value then
            None
        else
            let match' = regex.Match(value: string)

            if match'.Success then
                Some match'
            else
                None

    let (|IsMatch|_|) (regex: Regex) =
        function
        | Match regex match' -> Some match'.Value
        | _ -> None

    let (|IsNotMatch|_|) (regex: Regex) =
        function
        | IsMatch regex _ -> None
        | value -> Some value

    let private getGroupValueOrDefault (group: Group) =
        match group with
        | group when group.Success -> Some group.Value
        | _ -> None

    let (|MatchOneGroupOrDefault|_|) (regex: Regex) groupName =
        function
        | Match regex value -> Some(getGroupValueOrDefault value.Groups.[groupName: string])
        | _ -> None

    let (|MatchOneGroup|_|) (regex: Regex) groupName =
        function
        | MatchOneGroupOrDefault regex groupName group ->
            match group with
            | Some value -> Some value
            | None -> None
        | _ -> None

    let (|MatchTwoGroupsOrDefault|_|) (regex: Regex) group1Name group2Name =
        function
        | Match regex value ->
            let group1 =
                getGroupValueOrDefault value.Groups.[group1Name: string]

            let group2 =
                getGroupValueOrDefault value.Groups.[group2Name: string]

            Some(group1, group2)
        | _ -> None

    let (|MatchTwoGroups|_|) (regex: Regex) group1Name group2Name =
        function
        | MatchTwoGroupsOrDefault regex group1Name group2Name (group1, group2) ->
            match group1, group2 with
            | Some value1, Some value2 -> Some(value1, value2)
            | _ -> None
        | _ -> None

    let (|MatchGroupsOrDefault|_|) (regex: Regex) groupNames =
        function
        | Match regex value ->
            let groups =
                groupNames
                |> Seq.map (fun n -> getGroupValueOrDefault value.Groups.[n: string])
                |> Seq.toList

            Some groups
        | _ -> None

    let (|MatchGroups|_|) (regex: Regex) groupNames =
        function
        | MatchGroupsOrDefault regex groupNames groups ->
            let rec loop groups =
                match groups with
                | [] -> Some []
                | Some x :: rest ->
                    match loop rest with
                    | Some rest -> Some(x :: rest)
                    | None -> None
                | None :: _ -> None

            loop groups
        | _ -> None

    let private compiledRegex (pattern: string) = Regex(pattern, RegexOptions.Compiled)

    let (|Pattern|_|) (pattern: string) =
        function
        | Match (compiledRegex pattern) match' -> Some match'
        | _ -> None

    let (|IsPattern|_|) (pattern: string) =
        function
        | IsMatch (compiledRegex pattern) match' -> Some match'
        | _ -> None

    let (|IsNotPattern|_|) (pattern: string) =
        function
        | IsNotMatch (compiledRegex pattern) value -> Some value
        | _ -> None

    let (|PatternOneGroupOrDefault|_|) (pattern: string) groupName =
        function
        | MatchOneGroupOrDefault (compiledRegex pattern) groupName group -> Some group
        | _ -> None

    let (|PatternOneGroup|_|) (pattern: string) groupName =
        function
        | MatchOneGroup (compiledRegex pattern) groupName group -> Some group
        | _ -> None

    let (|PatternTwoGroupsOrDefault|_|) (pattern: string) group1Name group2Name =
        function
        | MatchTwoGroupsOrDefault (compiledRegex pattern) group1Name group2Name groups -> Some groups
        | _ -> None

    let (|PatternTwoGroups|_|) (pattern: string) group1Name group2Name =
        function
        | MatchTwoGroups (compiledRegex pattern) group1Name group2Name groups -> Some groups
        | _ -> None

    let (|PatternGroupsOrDefault|_|) (pattern: string) groupNames =
        function
        | MatchGroupsOrDefault (compiledRegex pattern) groupNames groups -> Some groups
        | _ -> None

    let (|PatternGroups|_|) (pattern: string) groupNames =
        function
        | MatchGroups (compiledRegex pattern) groupNames groups -> Some groups
        | _ -> None
