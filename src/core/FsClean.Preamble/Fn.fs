namespace FsClean

type Fn<'a, 'b> = 'a -> 'b

[<RequireQualifiedAccess>]
module Fn =
    let id<'a> : Fn<'a, 'a> = id

    let konst k : Fn<'a, 'b> = konst k

    let pipeTo (second: Fn<'b, 'c>) (first: Fn<'a, 'b>) : Fn<'a, 'c> = first >> second

    let ofMap map : Fn<'a, 'b> = fun key -> Map.find key map

    let ofDict map : Fn<'a, 'b> = fun key -> Dict.find key map
