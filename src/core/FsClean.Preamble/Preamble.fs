[<AutoOpen>]
module FsClean.Preamble

let flip fn = fun x y -> fn y x
let tee fn x = fn x; x

let clamp a b v = if v < a then a else if v > b then b else v
let clampMin a v = if v < a then a else v
let clampMax a v = if v > a then a else v

let notImplemented () = raise (System.NotImplementedException())
