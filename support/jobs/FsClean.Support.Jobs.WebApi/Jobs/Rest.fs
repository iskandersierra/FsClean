namespace FsClean.Support.Jobs.WebApi.Jobs

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

[<ApiController>]
[<Route("api/jobs")>]
type JobsController (logger : ILogger<JobsController>) =
    inherit ControllerBase()

    [<HttpPost("", Name = "JobCreate")>]
    member _.JobCreate() =
        [|1;2;3;4|]
