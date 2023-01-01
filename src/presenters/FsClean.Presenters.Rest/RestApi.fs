[<RequireQualifiedAccess>]
module FsClean.Presenters.Rest.RestApi

open System
open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc

open FsClean.Domain

module ActionResult =

    type ActionData =
        { action: string
          controller: string
          values: obj }

    type RouteData = { name: string; values: obj }

    let setProblemDetails (error: DomainError) (problem: #ProblemDetails) =
        problem.Title <- error.code
        problem.Detail <- error.description

        match error.service with
        | Some service -> problem.Extensions.["service"] <- service
        | None -> ()

        match error.entity with
        | Some entity -> problem.Extensions.["entity"] <- entity
        | None -> ()

        match error.operation with
        | Some operation -> problem.Extensions.["operation"] <- operation
        | None -> ()

        match error.entityId with
        | Some entityId -> problem.Extensions.["entityId"] <- entityId
        | None -> ()

        problem

    let problemDetails (error: DomainError) =
        new ProblemDetails() |> setProblemDetails error

    let validationProblemDetails (error: DomainError) =
        new ValidationProblemDetails()
        |> setProblemDetails error

    let toError (error: DomainError) =
        match error.errorData with
        | Failure ->
            let problem = problemDetails error
            problem.Status <- 500
            let result = ObjectResult(problem)
            result.StatusCode <- 500
            result
        | Unexpected ->
            let problem = problemDetails error
            problem.Status <- 500
            let result = ObjectResult(problem)
            result.StatusCode <- 500
            result
        | NotFound ->
            let problem = problemDetails error
            problem.Status <- 404
            NotFoundObjectResult(problem)
        | Unauthorized ->
            let problem = problemDetails error
            problem.Status <- 401
            UnauthorizedObjectResult(problem)
        | Validation errors ->
            let problem = validationProblemDetails error
            problem.Status <- 400

            errors
            |> Map.iter (fun key value -> problem.Errors.Add(key, value |> Array.ofList))

            BadRequestObjectResult(problem)
        | Conflict errors ->
            let problem = validationProblemDetails error
            problem.Status <- 409

            errors
            |> Map.iter (fun key value -> problem.Errors.Add(key, value |> Array.ofList))

            ConflictObjectResult(problem)

    let toResult fn (result: Task<DomainResult<'a>>) =
        task {
            match! result with
            | Ok result -> return fn (result) :> IActionResult
            | Error error -> return toError error
        }

    let toOk result = result |> toResult OkObjectResult
    let toOkEmpty result = result |> toResult OkResult

    let toCreated result =
        result
        |> toResult (fun (r, l) -> CreatedResult((l: string), r))

    let toCreatedUri result =
        result
        |> toResult (fun (r, l) -> CreatedResult((l: Uri), r))

    let toCreatedAction (result: Task<DomainResult<'a * ActionData>>) =
        result
        |> toResult (fun (r, d) -> CreatedAtActionResult(d.action, d.controller, d.values, r))

    let toCreatedRoute (result: Task<DomainResult<'a * RouteData>>) =
        result
        |> toResult (fun (r, d) -> CreatedAtRouteResult(d.name, d.values, r))

    let toAcceptedEmpty result =
        result |> toResult (fun () -> AcceptedResult())

    let toAccepted result =
        result
        |> toResult (fun (r, l) -> AcceptedResult((l: string), r))

    let toAcceptedUri result =
        result
        |> toResult (fun (r, l) -> AcceptedResult((l: Uri), r))

    let toAcceptedAction (result: Task<DomainResult<'a * ActionData>>) =
        result
        |> toResult (fun (r, d) -> AcceptedAtActionResult(d.action, d.controller, d.values, r))

    let toAcceptedRoute (result: Task<DomainResult<'a * RouteData>>) =
        result
        |> toResult (fun (r, d) -> AcceptedAtRouteResult(d.name, d.values, r))
