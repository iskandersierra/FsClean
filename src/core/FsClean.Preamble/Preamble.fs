[<AutoOpen>]
module FsClean.Preamble

let flip fn = fun x y -> fn y x
let tee fn x = fn x; x

let konst v = fun _ -> v
let konst2 v = fun _ _ -> v
let konst3 v = fun _ _ _ -> v

let curry fn = fun x y -> fn (x, y)
let uncurry fn = fun (x, y) -> fn x y

let getType (o: #obj) = o.GetType()
let toString (o: #obj) = o.ToString()

let clamp a b v = if v < a then a else if v > b then b else v
let clampMin a v = if v < a then a else v
let clampMax a v = if v > a then a else v

let notImplemented () = raise (System.NotImplementedException())

let asPattern predicate =
    fun value ->
        if predicate value then
            Some value
        else
            None
