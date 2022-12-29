[<AutoOpen>]
module FsClean.Preamble

let flip fn = fun x y -> fn y x

let notImplemented () = raise (System.NotImplementedException())
