[<AutoOpen>]
module FsClean.Preamble

open System.Text.RegularExpressions

// String active patterns

let (|StringIsNullOrEmpty|_|) value =
    if String.isNullOrEmpty value then
        Some()
    else
        None

let (|StringIsNullOrWhiteSpace|_|) value =
    if String.isNullOrWhiteSpace value then
        Some()
    else
        None

let (|StringHasLengthOutside|_|) minLength maxLength value =
    let length = String.length value

    if length < minLength || length > maxLength then
        Some value
    else
        None

let (|StringHasLengthBetween|_|) minLength maxLength value =
    let length = String.length value

    if length >= minLength && length <= maxLength then
        Some value
    else
        None

let (|StringHasLength|_|) length value =
    if String.length value = length then
        Some value
    else
        None

let (|StringIsMatch|_|) (regex: Regex) value =
    if not (isNull value) && regex.IsMatch(value: string) then
        Some value
    else
        None

let (|StringIsNotMatch|_|) (regex: Regex) value =
    if isNull value || not (regex.IsMatch(value: string)) then
        Some ()
    else
        None

let (|StringMatch|_|) (regex: Regex) value =
    if isNull value then
        None
    else
        let match' = regex.Match(value: string)

        if match'.Success then
            Some match'
        else
            None

let private getGroupValueOrDefault (group: Group) =
    match group with
    | group when group.Success -> Some group.Value
    | _ -> None

let (|StringMatchOneGroupOrDefault|_|) (regex: Regex) groupName =
    function
    | StringMatch regex value -> Some(getGroupValueOrDefault value.Groups.[groupName: string])
    | _ -> None

let (|StringMatchOneGroup|_|) (regex: Regex) groupName =
    function
    | StringMatchOneGroupOrDefault regex groupName group ->
        match group with
        | Some value -> Some value
        | None -> None
    | _ -> None

let (|StringMatchTwoGroupsOrDefault|_|) (regex: Regex) group1Name group2Name =
    function
    | StringMatch regex value ->
        let group1 =
            getGroupValueOrDefault value.Groups.[group1Name: string]

        let group2 =
            getGroupValueOrDefault value.Groups.[group2Name: string]

        Some(group1, group2)
    | _ -> None

let (|StringMatchTwoGroups|_|) (regex: Regex) group1Name group2Name =
    function
    | StringMatchTwoGroupsOrDefault regex group1Name group2Name (group1, group2) ->
        match group1, group2 with
        | Some value1, Some value2 -> Some(value1, value2)
        | _ -> None
    | _ -> None

let (|StringMatchGroupsOrDefault|_|) (regex: Regex) groupNames =
    function
    | StringMatch regex value ->
        let groups =
            groupNames
            |> Seq.map (fun n -> getGroupValueOrDefault value.Groups.[n: string])
            |> Seq.toList

        Some groups
    | _ -> None

let (|StringMatchGroups|_|) (regex: Regex) groupNames =
    function
    | StringMatchGroupsOrDefault regex groupNames groups ->
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
