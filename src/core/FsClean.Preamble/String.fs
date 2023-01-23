/// module String defines functions and types for string manipulation.
[<RequireQualifiedAccess>]
module FsClean.String

open System
open System.Text
open System.Text.RegularExpressions

/// isNullOrEmpty returns true if the given string is null or empty.
let isNullOrEmpty (value: string) = String.IsNullOrEmpty value
/// isNullOrWhiteSpace returns true if the given string is null or white space.
let isNullOrWhiteSpace (value: string) = String.IsNullOrWhiteSpace value

/// isNotNullOrEmpty returns true if the given string is not null or empty.
let isNotNullOrEmpty = isNullOrEmpty >> not
/// isNotNullOrWhiteSpace returns true if the given string is not null or white space.
let isNotNullOrWhiteSpace = isNullOrWhiteSpace >> not

/// trim returns a string with leading and trailing white space removed.
let trim (str: string) = str.Trim()
/// trimStart returns a string with leading white space removed.
let trimStart (str: string) = str.TrimStart()
/// trimEnd returns a string with trailing white space removed.
let trimEnd (str: string) = str.TrimEnd()

/// trimChar returns a string with leading and trailing occurrences of the given character removed.
let trimChar ch (str: string) = str.Trim(ch: char)
/// trimStartChar returns a string with leading occurrences of the given character removed.
let trimStartChar ch (str: string) = str.TrimStart(ch: char)
/// trimEndChar returns a string with trailing occurrences of the given character removed.
let trimEndChar ch (str: string) = str.TrimEnd(ch: char)

/// trimChars returns a string with leading and trailing occurrences of the given characters removed.
let trimChars ch (str: string) = str.Trim(ch: char [])
/// trimStartChars returns a string with leading occurrences of the given characters removed.
let trimStartChars ch (str: string) = str.TrimStart(ch: char [])
/// trimEndChars returns a string with trailing occurrences of the given characters removed.
let trimEndChars ch (str: string) = str.TrimEnd(ch: char [])

/// trimOrNull returns a string with leading and trailing white space removed, or null if the given string is null.
let trimOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trim str

/// trimStartOrNull returns a string with leading white space removed, or null if the given string is null.
let trimStartOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trimStart str

/// trimEndOrNull returns a string with trailing white space removed, or null if the given string is null.
let trimEndOrNull (str: string) =
    match str with
    | null -> null
    | _ -> trimEnd str

/// trimOrEmpty returns a string with leading and trailing white space removed, or the empty string if the given string is null.
let trimOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trim str

/// trimStartOrEmpty returns a string with leading white space removed, or the empty string if the given string is null.
let trimStartOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trimStart str

/// trimEndOrEmpty returns a string with trailing white space removed, or the empty string if the given string is null.
let trimEndOrEmpty (str: string) =
    match str with
    | null -> ""
    | _ -> trimEnd str

/// startsWith returns true if the given string starts with the given prefix.
let startsWith prefix (value: string) = value.StartsWith(prefix: string)
/// startsWithChar returns true if the given string starts with the given prefix.
let startsWithChar prefix (value: string) = value.StartsWith(prefix: char)
/// startsWithComparison returns true if the given string starts with the given prefix, using the given comparison.
let startsWithComparison comparison prefix (value: string) = value.StartsWith(prefix, comparison)

/// startsWithCulture returns true if the given string starts with the given prefix, using the given culture and case sensitivity.
let startsWithCulture culture ignoreCase prefix (value: string) =
    value.StartsWith(prefix, ignoreCase, culture)

/// endsWith returns true if the given string ends with the given prefix.
let endsWith prefix (value: string) = value.EndsWith(prefix: string)
/// endsWithChar returns true if the given string ends with the given prefix.
let endsWithChar prefix (value: string) = value.EndsWith(prefix: char)
/// endsWithComparison returns true if the given string ends with the given prefix, using the given comparison.
let endsWithComparison comparison prefix (value: string) = value.EndsWith(prefix, comparison)

/// endsWithCulture returns true if the given string ends with the given prefix, using the given culture and case sensitivity.
let endsWithCulture culture ignoreCase prefix (value: string) =
    value.EndsWith(prefix, ignoreCase, culture)

/// withStarting returns the given string with the given prefix added if it does not already start with the prefix.
let withStarting prefix (value: string) =
    if startsWith prefix value then
        value
    else
        prefix + value

/// withStartingChar returns the given string with the given prefix added if it does not already start with the prefix.
let withStartingChar prefix (value: string) =
    if startsWithChar prefix value then
        value
    else
        string prefix + value

/// withEnding returns the given string with the given prefix added if it does not already end with the prefix.
let withEnding prefix (value: string) =
    if endsWith prefix value then
        value
    else
        prefix + value

/// withEndingChar returns the given string with the given prefix added if it does not already end with the prefix.
let withEndingChar prefix (value: string) =
    if endsWithChar prefix value then
        value
    else
        string prefix + value

// Encoding

/// toEncodingBytes returns the given string encoded using the given encoding.
let toEncodingBytes (encoding: Encoding) (str: string) = encoding.GetBytes str
/// ofEncodingBytes returns the given bytes decoded using the given encoding.
let ofEncodingBytes (encoding: Encoding) (bytes: byte []) = encoding.GetString bytes

/// toUtf8 returns the given string encoded using UTF8.
let toUtf8 = toEncodingBytes Encoding.UTF8
/// ofUtf8 returns the given bytes decoded using UTF8.
let ofUtf8 = ofEncodingBytes Encoding.UTF8

/// module Patterns provides active patterns for working with strings.
module Patterns =
    /// IsNullOrEmpty matches a null or empty string. The input string is returned.
    let (|IsNullOrEmpty|_|) = asPattern isNullOrEmpty
    /// IsNullOrWhiteSpace matches a null or white space string. The input string is returned.
    let (|IsNullOrWhiteSpace|_|) = asPattern isNullOrWhiteSpace

    /// IsNotNullOrEmpty matches a non-null or empty string. The input string is returned.
    let (|IsNotNullOrEmpty|_|) = asPattern isNotNullOrEmpty
    /// IsNotNullOrWhiteSpace matches a non-null or white space string. The input string is returned.
    let (|IsNotNullOrWhiteSpace|_|) = asPattern isNotNullOrWhiteSpace

    /// HasLengthOutside matches a string with a length outside the given range. The input string is returned.
    let (|HasLengthOutside|_|) minLength maxLength =
        asPattern (fun value ->
            String.length value < minLength
            || String.length value > maxLength)

    /// HasLengthBetween matches a string with a length between the given range. The input string is returned.
    let (|HasLengthBetween|_|) minLength maxLength =
        asPattern (fun value ->
            String.length value >= minLength
            && String.length value <= maxLength)

    /// HasLength matches a string with the given length. The input string is returned.
    let (|HasLength|_|) length value =
        asPattern (String.length >> (=) length) value

    /// Match matches a string against the given regular expression. The match is returned.
    let (|Match|_|) (regex: Regex) value =
        if isNull value then
            None
        else
            let match' = regex.Match(value: string)

            if match'.Success then
                Some match'
            else
                None

    /// IsMatch matches a string against the given regular expression. The matched string is returned.
    let (|IsMatch|_|) (regex: Regex) =
        function
        | Match regex match' -> Some match'.Value
        | _ -> None

    /// IsNotMatch matches a string against the given regular expression. The input string is returned.
    let (|IsNotMatch|_|) (regex: Regex) =
        function
        | IsMatch regex _ -> None
        | value -> Some value

    let private getGroupValueOrDefault (group: Group) =
        match group with
        | group when group.Success -> Some group.Value
        | _ -> None

    /// MatchOneGroupOrDefault matches a string against the given regular expression and returns the value of the given group, if it exists.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// If the group is not found, None is returned.
    /// If the group exists, the value of the group is returned.
    let (|MatchOneGroupOrDefault|_|) (regex: Regex) groupName =
        function
        | Match regex value -> Some(getGroupValueOrDefault value.Groups.[groupName: string])
        | _ -> None

    /// MatchOneGroup matches a string against the given regular expression and returns the value of the given group, if it exists.
    /// If the string does not match the regular expression or the group is not found, the active pattern does not match.
    /// If the group exists, the value of the group is returned.
    let (|MatchOneGroup|_|) (regex: Regex) groupName =
        function
        | MatchOneGroupOrDefault regex groupName group ->
            match group with
            | Some value -> Some value
            | None -> None
        | _ -> None

    /// MatchTwoGroupsOrDefault matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// If not all groups are found, None is returned.
    /// If all groups exist, the values of the groups are returned.
    let (|MatchTwoGroupsOrDefault|_|) (regex: Regex) group1Name group2Name =
        function
        | Match regex value ->
            let group1 =
                getGroupValueOrDefault value.Groups.[group1Name: string]

            let group2 =
                getGroupValueOrDefault value.Groups.[group2Name: string]

            Some(group1, group2)
        | _ -> None

    /// MatchTwoGroups matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression or not all groups are found, the active pattern does not match.
    /// If all groups exist, the values of the groups are returned.
    let (|MatchTwoGroups|_|) (regex: Regex) group1Name group2Name =
        function
        | MatchTwoGroupsOrDefault regex group1Name group2Name (group1, group2) ->
            match group1, group2 with
            | Some value1, Some value2 -> Some(value1, value2)
            | _ -> None
        | _ -> None

    /// MatchGroupsOrDefault matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// A list of group matches is returned, where for each group, a None is returned if the group is not found, and the value of the group is returned if it exists.
    let (|MatchGroupsOrDefault|_|) (regex: Regex) groupNames =
        function
        | Match regex value ->
            let groups =
                groupNames
                |> Seq.map (fun n -> getGroupValueOrDefault value.Groups.[n: string])
                |> Seq.toList

            Some groups
        | _ -> None

    /// MatchGroups matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression or not all groups are found, the active pattern does not match.
    /// A list of group matches is returned, where for each group, the value of the group is returned on the same position as the group name in the list of group names.
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

    /// Pattern matches a string against the given regular expression. The match is returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|Pattern|_|) (pattern: string) =
        function
        | Match (compiledRegex pattern) match' -> Some match'
        | _ -> None

    /// IsPattern matches a string against the given regular expression. The match is returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|IsPattern|_|) (pattern: string) =
        function
        | IsMatch (compiledRegex pattern) match' -> Some match'
        | _ -> None

    /// IsNotPattern matches a string against the given regular expression. The input string is returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|IsNotPattern|_|) (pattern: string) =
        function
        | IsNotMatch (compiledRegex pattern) value -> Some value
        | _ -> None

    /// PatternOneGroupOrDefault matches a string against the given regular expression and returns the value of the given group, if it exists.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// If the group is not found, None is returned.
    /// If the group exists, the value of the group is returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternOneGroupOrDefault|_|) (pattern: string) groupName =
        function
        | MatchOneGroupOrDefault (compiledRegex pattern) groupName group -> Some group
        | _ -> None

    /// PatternOneGroup matches a string against the given regular expression and returns the value of the given group, if it exists.
    /// If the string does not match the regular expression or the group is not found, the active pattern does not match.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternOneGroup|_|) (pattern: string) groupName =
        function
        | MatchOneGroup (compiledRegex pattern) groupName group -> Some group
        | _ -> None

    /// PatternTwoGroupsOrDefault matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// If not all groups are found, None is returned.
    /// If all groups exist, the values of the groups are returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternTwoGroupsOrDefault|_|) (pattern: string) group1Name group2Name =
        function
        | MatchTwoGroupsOrDefault (compiledRegex pattern) group1Name group2Name groups -> Some groups
        | _ -> None

    /// PatternTwoGroups matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression or not all groups are found, the active pattern does not match.
    /// If all groups exist, the values of the groups are returned.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternTwoGroups|_|) (pattern: string) group1Name group2Name =
        function
        | MatchTwoGroups (compiledRegex pattern) group1Name group2Name groups -> Some groups
        | _ -> None

    /// PatternGroupsOrDefault matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression, the active pattern does not match.
    /// A list of group matches is returned, where for each group, a None is returned if the group is not found, and the value of the group is returned if it exists.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternGroupsOrDefault|_|) (pattern: string) groupNames =
        function
        | MatchGroupsOrDefault (compiledRegex pattern) groupNames groups -> Some groups
        | _ -> None

    /// PatternGroups matches a string against the given regular expression and returns the value of the given groups, if they exist.
    /// If the string does not match the regular expression or not all groups are found, the active pattern does not match.
    /// A list of group matches is returned, where for each group, the value of the group is returned on the same position as the group name in the list of group names.
    /// The regular expression given as a string pattern and compiled to make it faster.
    let (|PatternGroups|_|) (pattern: string) groupNames =
        function
        | MatchGroups (compiledRegex pattern) groupNames groups -> Some groups
        | _ -> None

    /// StartsWith matches a string against the given prefix. The input string is returned.
    let (|StartsWith|_|) prefix = asPattern (startsWith prefix)
    /// StartsWithChar matches a string against the given prefix. The input string is returned.
    let (|StartsWithChar|_|) prefix = asPattern (startsWithChar prefix)

    /// EndsWith matches a string against the given suffix. The input string is returned.
    let (|EndsWith|_|) prefix = asPattern (endsWith prefix)
    /// EndsWithChar matches a string against the given suffix. The input string is returned.
    let (|EndsWithChar|_|) prefix = asPattern (endsWithChar prefix)
