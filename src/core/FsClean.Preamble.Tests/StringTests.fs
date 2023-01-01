module FsClean.StringTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit
open FsClean


[<Theory>]
[<InlineData(null, true)>]
[<InlineData("", true)>]
[<InlineData(" ", false)>]
[<InlineData("Abc", false)>]
let ``isNullOrEmpty should work`` (value: string, expected: bool) =
    let result = String.isNullOrEmpty value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, true)>]
[<InlineData("", true)>]
[<InlineData(" \t\r\n ", true)>]
[<InlineData("Abc", false)>]
let ``isNullOrWhiteSpace should work`` (value: string, expected: bool) =
    let result = String.isNullOrWhiteSpace value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData(" ", true)>]
[<InlineData("Abc", true)>]
let ``isNotNullOrEmpty should work`` (value: string, expected: bool) =
    let result = String.isNotNullOrEmpty value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData(" \t\r\n ", false)>]
[<InlineData("Abc", true)>]
let ``isNotNullOrWhiteSpace should work`` (value: string, expected: bool) =
    let result = String.isNotNullOrWhiteSpace value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trim should work`` (value: string, expected: string) =
    let result = String.trim value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimStart should work`` (value: string, expected: string) =
    let result = String.trimStart value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", " \t\r\n Title of book")>]
[<InlineData(" \t\r\n Title of book", " \t\r\n Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimEnd should work`` (value: string, expected: string) =
    let result = String.trimEnd value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("bbbbbbTitle of bookbbbbbb", "Title of book")>]
[<InlineData("bbbbbbTitle of book", "Title of book")>]
[<InlineData("Title of bookbbbbbb", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimChar should work`` (value: string, expected: string) =
    let result = String.trimChar 'b' value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("bbbbbbTitle of bookbbbbbb", "Title of bookbbbbbb")>]
[<InlineData("bbbbbbTitle of book", "Title of book")>]
[<InlineData("Title of bookbbbbbb", "Title of bookbbbbbb")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimStartChar should work`` (value: string, expected: string) =
    let result = String.trimStartChar 'b' value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("bbbbbbTitle of bookbbbbbb", "bbbbbbTitle of book")>]
[<InlineData("bbbbbbTitle of book", "bbbbbbTitle of book")>]
[<InlineData("Title of bookbbbbbb", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimEndChar should work`` (value: string, expected: string) =
    let result = String.trimEndChar 'b' value
    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("abcabcTitle of bookabcabc", "Title of book")>]
[<InlineData("abcabcTitle of book", "Title of book")>]
[<InlineData("Title of bookabcabc", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimChars should work`` (value: string, expected: string) =
    let result =
        String.trimChars ("abc".ToCharArray()) value

    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("abcabcTitle of bookabcabc", "Title of bookabcabc")>]
[<InlineData("abcabcTitle of book", "Title of book")>]
[<InlineData("Title of bookabcabc", "Title of bookabcabc")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimStartChars should work`` (value: string, expected: string) =
    let result =
        String.trimStartChars ("abc".ToCharArray()) value

    test <@ result = expected @>

[<Theory>]
[<InlineData("", "")>]
[<InlineData("abcabcTitle of bookabcabc", "abcabcTitle of book")>]
[<InlineData("abcabcTitle of book", "abcabcTitle of book")>]
[<InlineData("Title of bookabcabc", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimEndChars should work`` (value: string, expected: string) =
    let result =
        String.trimEndChars ("abc".ToCharArray()) value

    test <@ result = expected @>

// Test trimOrNull and trimOrEmpty

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimOrNull should work`` (value: string, expected: string) =
    let result = String.trimOrNull value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimStartOrNull should work`` (value: string, expected: string) =
    let result = String.trimStartOrNull value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", " \t\r\n Title of book")>]
[<InlineData(" \t\r\n Title of book", " \t\r\n Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimEndOrNull should work`` (value: string, expected: string) =
    let result = String.trimEndOrNull value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, "")>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimOrEmpty should work`` (value: string, expected: string) =
    let result = String.trimOrEmpty value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, "")>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData(" \t\r\n Title of book", "Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book \t\r\n ")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimStartOrEmpty should work`` (value: string, expected: string) =
    let result = String.trimStartOrEmpty value
    test <@ result = expected @>

[<Theory>]
[<InlineData(null, "")>]
[<InlineData("", "")>]
[<InlineData("    ", "")>]
[<InlineData(" \t\r\n Title of book \t\r\n ", " \t\r\n Title of book")>]
[<InlineData(" \t\r\n Title of book", " \t\r\n Title of book")>]
[<InlineData("Title of book \t\r\n ", "Title of book")>]
[<InlineData("Title of book", "Title of book")>]
let ``trimEndOrEmpty should work`` (value: string, expected: string) =
    let result = String.trimEndOrEmpty value
    test <@ result = expected @>



open System.Text.RegularExpressions
open FsClean.String.Patterns

[<Theory>]
[<InlineData(null, true)>]
[<InlineData("", true)>]
[<InlineData(" ", false)>]
[<InlineData("Abc", false)>]
let ``Patterns.IsNullOrEmpty should work`` (value: string, expected: bool) =
    match value with
    | IsNullOrEmpty value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, true)>]
[<InlineData("", true)>]
[<InlineData(" ", true)>]
[<InlineData("Abc", false)>]
let ``Patterns.IsNullOrWhiteSpace should work`` (value: string, expected: bool) =
    match value with
    | IsNullOrWhiteSpace value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData(" ", true)>]
[<InlineData("Abc", true)>]
let ``Patterns.IsNotNullOrEmpty should work`` (value: string, expected: bool) =
    match value with
    | IsNotNullOrEmpty value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData(" ", false)>]
[<InlineData("Abc", true)>]
let ``Patterns.IsNotNullOrWhiteSpace should work`` (value: string, expected: bool) =
    match value with
    | IsNotNullOrWhiteSpace value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData("", 0, 10, false)>]
[<InlineData("", 1, 10, true)>]
[<InlineData("abc", 0, 10, false)>]
[<InlineData("abc", 3, 10, false)>]
[<InlineData("abc", 4, 10, true)>]
[<InlineData("abc", 0, 3, false)>]
[<InlineData("abc", 0, 2, true)>]
let ``Patterns.HasLengthOutside should work`` (value: string, min: int, max: int, expected: bool) =
    match value with
    | HasLengthOutside min max value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData("", 0, 10, true)>]
[<InlineData("", 1, 10, false)>]
[<InlineData("abc", 0, 10, true)>]
[<InlineData("abc", 3, 10, true)>]
[<InlineData("abc", 4, 10, false)>]
[<InlineData("abc", 0, 3, true)>]
[<InlineData("abc", 0, 2, false)>]
let ``Patterns.HasLengthBetween should work`` (value: string, min: int, max: int, expected: bool) =
    match value with
    | HasLengthBetween min max value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData("", 0, true)>]
[<InlineData("", 1, false)>]
[<InlineData("abc", 0, false)>]
[<InlineData("abc", 3, true)>]
[<InlineData("abc", 4, false)>]
let ``Patterns.HasLength should work`` (value: string, length: int, expected: bool) =
    match value with
    | HasLength length value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>



[<Literal>]
let private DatePattern =
    "(?<year>\d{4})-(?<month>\d{2})(-(?<day>\d{2}))?"

[<Theory>]
[<InlineData(null, DatePattern, null, false)>]
[<InlineData("", DatePattern, null, false)>]
[<InlineData("2022-12-22", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "2022-12", true)>]
[<InlineData("2022/12/22", DatePattern, null, false)>]
let ``Patterns.IsMatch should work`` (value: string, pattern: string, expectedValue: string, expected: bool) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | IsMatch regex value' ->
        test <@ expected = true @>
        test <@ value' = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, true)>]
[<InlineData("", DatePattern, true)>]
[<InlineData("2022-12-22", DatePattern, false)>]
[<InlineData("Born in 2022-12-22.", DatePattern, false)>]
[<InlineData("Born in 2022-12.", DatePattern, false)>]
[<InlineData("2022/12/22", DatePattern, true)>]
let ``Patterns.IsNotMatch should work`` (value: string, pattern: string, expected: bool) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | IsNotMatch regex value' ->
        test <@ expected = true @>
        test <@ value' = value @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, null, false)>]
[<InlineData("", DatePattern, null, false)>]
[<InlineData("2022-12-22", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "2022-12", true)>]
[<InlineData("2022/12/22", DatePattern, null, false)>]
let ``Patterns.Match should work`` (value: string, pattern: string, expectedValue: string, expected: bool) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | Match regex match' ->
        test <@ expected = true @>
        test <@ match'.Success = true @>
        test <@ match'.Value = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", null, false)>]
[<InlineData("", DatePattern, "year", null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "2022", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "month", "12", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "day", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", null, false)>]
let ``Patterns.MatchOneGroupOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName: string,
        expectedValue: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchOneGroupOrDefault regex groupName group ->
        test <@ expected = true @>
        test <@ group = Option.ofObj expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", null, false)>]
[<InlineData("", DatePattern, "year", null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "2022", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "month", "12", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "day", null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", null, false)>]
let ``Patterns.MatchOneGroup should work``
    (
        value: string,
        pattern: string,
        groupName: string,
        expectedValue: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchOneGroup regex groupName group ->
        test <@ expected = true @>
        test <@ group = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "day", null, null, false)>]
[<InlineData("", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "day", "2022", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", "day", null, null, false)>]
let ``Patterns.MatchTwoGroupsOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        expectedValue1: string,
        expectedValue2: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchTwoGroupsOrDefault regex groupName1 groupName2 (group1, group2) ->
        test <@ expected = true @>
        test <@ group1 = Option.ofObj expectedValue1 @>
        test <@ group2 = Option.ofObj expectedValue2 @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "day", null, null, false)>]
[<InlineData("", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", "day", null, null, false)>]
let ``Patterns.MatchTwoGroups should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        expectedValue1: string,
        expectedValue2: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchTwoGroups regex groupName1 groupName2 (group1, group2) ->
        test <@ expected = true @>
        test <@ group1 = expectedValue1 @>
        test <@ group2 = expectedValue2 @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "month", "day", "2022", "12", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", "month", "day", null, null, null, false)>]
let ``Patterns.MatchGroupsOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        groupName3: string,
        expectedValue1: string,
        expectedValue2: string,
        expectedValue3: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchGroupsOrDefault regex [ groupName1; groupName2; groupName3 ] groups ->
        test <@ expected = true @>

        let expectedList =
            [ expectedValue1
              expectedValue2
              expectedValue3 ]
            |> List.map Option.ofObj

        test <@ groups = expectedList @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", "month", "day", null, null, null, false)>]
let ``Patterns.MatchGroups should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        groupName3: string,
        expectedValue1: string,
        expectedValue2: string,
        expectedValue3: string,
        expected: bool
    ) =
    let regex = Regex(pattern, RegexOptions.Compiled)

    match value with
    | MatchGroups regex [ groupName1; groupName2; groupName3 ] groups ->
        test <@ expected = true @>

        let expectedList =
            [ expectedValue1
              expectedValue2
              expectedValue3 ]

        test <@ groups = expectedList @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>


[<Theory>]
[<InlineData(null, DatePattern, null, false)>]
[<InlineData("", DatePattern, null, false)>]
[<InlineData("2022-12-22", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "2022-12", true)>]
[<InlineData("2022/12/22", DatePattern, null, false)>]
let ``Patterns.Pattern should work`` (value: string, pattern: string, expectedValue: string, expected: bool) =
    match value with
    | Pattern pattern match' ->
        test <@ expected = true @>
        test <@ match'.Success = true @>
        test <@ match'.Value = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, null, false)>]
[<InlineData("", DatePattern, null, false)>]
[<InlineData("2022-12-22", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "2022-12-22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "2022-12", true)>]
[<InlineData("2022/12/22", DatePattern, null, false)>]
let ``Patterns.IsPattern should work`` (value: string, pattern: string, expectedValue: string, expected: bool) =
    match value with
    | IsPattern pattern value' ->
        test <@ expected = true @>
        test <@ value' = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, null, true)>]
[<InlineData("", DatePattern, "", true)>]
[<InlineData("2022-12-22", DatePattern, "2022-12-22", false)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "2022-12-22", false)>]
[<InlineData("Born in 2022-12.", DatePattern, "2022-12", false)>]
[<InlineData("2022/12/22", DatePattern, "2022/12/22", true)>]
let ``Patterns.IsNotPattern should work`` (value: string, pattern: string, expectedValue: string, expected: bool) =
    match value with
    | IsNotPattern pattern value' ->
        test <@ expected = true @>
        test <@ value' = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", null, false)>]
[<InlineData("", DatePattern, "year", null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "2022", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "month", "12", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "day", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", null, false)>]
let ``Patterns.PatternOneGroupOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName: string,
        expectedValue: string,
        expected: bool
    ) =
    match value with
    | PatternOneGroupOrDefault pattern groupName group ->
        test <@ expected = true @>
        test <@ group = Option.ofObj expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", null, false)>]
[<InlineData("", DatePattern, "year", null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "2022", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "month", "12", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "day", null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", null, false)>]
let ``Patterns.PatternOneGroup should work``
    (
        value: string,
        pattern: string,
        groupName: string,
        expectedValue: string,
        expected: bool
    ) =
    match value with
    | PatternOneGroup pattern groupName group ->
        test <@ expected = true @>
        test <@ group = expectedValue @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "day", null, null, false)>]
[<InlineData("", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "day", "2022", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", "day", null, null, false)>]
let ``Patterns.PatternTwoGroupsOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        expectedValue1: string,
        expectedValue2: string,
        expected: bool
    ) =
    match value with
    | PatternTwoGroupsOrDefault pattern groupName1 groupName2 (group1, group2) ->
        test <@ expected = true @>
        test <@ group1 = Option.ofObj expectedValue1 @>
        test <@ group2 = Option.ofObj expectedValue2 @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "day", null, null, false)>]
[<InlineData("", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "day", "2022", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "day", null, null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", "day", null, null, false)>]
let ``Patterns.PatternTwoGroups should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        expectedValue1: string,
        expectedValue2: string,
        expected: bool
    ) =
    match value with
    | PatternTwoGroups pattern groupName1 groupName2 (group1, group2) ->
        test <@ expected = true @>
        test <@ group1 = expectedValue1 @>
        test <@ group2 = expectedValue2 @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "month", "day", "2022", "12", null, true)>]
[<InlineData("2022/12/22", DatePattern, "year", "month", "day", null, null, null, false)>]
let ``Patterns.PatternGroupsOrDefault should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        groupName3: string,
        expectedValue1: string,
        expectedValue2: string,
        expectedValue3: string,
        expected: bool
    ) =
    match value with
    | PatternGroupsOrDefault pattern [ groupName1; groupName2; groupName3 ] groups ->
        test <@ expected = true @>

        let expectedList =
            [ expectedValue1
              expectedValue2
              expectedValue3 ]
            |> List.map Option.ofObj

        test <@ groups = expectedList @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>

[<Theory>]
[<InlineData(null, DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022-12-22", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12-22.", DatePattern, "year", "month", "day", "2022", "12", "22", true)>]
[<InlineData("Born in 2022-12.", DatePattern, "year", "month", "day", null, null, null, false)>]
[<InlineData("2022/12/22", DatePattern, "year", "month", "day", null, null, null, false)>]
let ``Patterns.PatternGroups should work``
    (
        value: string,
        pattern: string,
        groupName1: string,
        groupName2: string,
        groupName3: string,
        expectedValue1: string,
        expectedValue2: string,
        expectedValue3: string,
        expected: bool
    ) =
    match value with
    | PatternGroups pattern [ groupName1; groupName2; groupName3 ] groups ->
        test <@ expected = true @>

        let expectedList =
            [ expectedValue1
              expectedValue2
              expectedValue3 ]

        test <@ groups = expectedList @>
    | value' ->
        test <@ expected = false @>
        test <@ value' = value @>
