/// module Exn defines functions and types for exception handling.
[<RequireQualifiedAccess>]
module FsClean.Exn

open System.Runtime.ExceptionServices

/// Rethrows an exception, preserving the original stack trace.
let inline rethrow exn =
    ExceptionDispatchInfo.Throw(exn)
    failwith "unreachable"
