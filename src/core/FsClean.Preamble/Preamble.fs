/// Preamble contains a collection of useful functions that are used throughout the FsClean codebase.
[<AutoOpen>]
module FsClean.Preamble

/// flip returns a function that takes its arguments in the reverse order.
let flip fn = fun x y -> fn y x

/// tee returns a unary function that applies the given function to its argument and returns the argument.
let tee fn = fun x -> fn x; x

/// konst returns a constant unary function that always returns the same value.
let konst v = fun _ -> v
/// konst2 returns a constant binary function that always returns the same value.
let konst2 v = fun _ _ -> v
/// konst3 returns a constant ternary function that always returns the same value.
let konst3 v = fun _ _ _ -> v

/// curry eturns a curried binary function from a function that takes a tuple.
let curry fn = fun x y -> fn (x, y)
/// uncurry eturns an function that takes a tuple from a curried binary function.
let uncurry fn = fun (x, y) -> fn x y
/// curry3 returns a curried ternary function from a function that takes a triple.
let curry3 fn = fun x y z -> fn (x, y, z)
/// uncurry3 returns a function that takes a triple from a curried ternary function.
let uncurry3 fn = fun (x, y, z) -> fn x y z

/// getType returns the type of an object.
let getType (o: #obj) = o.GetType()
/// toString returns the string representation of an object.
let toString (o: #obj) = o.ToString()

/// clamp returns a value that is within the given range.
let clamp a b v = if v < a then a else if v > b then b else v
/// clampMin returns a value that is greater than or equal to the given value.
let clampMin a v = if v < a then a else v
/// clampMax returns a value that is less than or equal to the given value.
let clampMax a v = if v > a then a else v

/// notImplemented raises a System.NotImplementedException.
let notImplemented () = raise (System.NotImplementedException())

/// asPattern returns a function that returns Some value if the predicate returns true, otherwise None.
/// This is useful for active patterns definitions.
let asPattern predicate =
    fun value ->
        if predicate value then
            Some value
        else
            None
