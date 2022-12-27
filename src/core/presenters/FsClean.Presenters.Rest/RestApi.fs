[<RequireQualifiedAccess>]
module FsClean.Presenters.Rest.RestApi

open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

open FsClean.Domain

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

let toActionResult (result: Task<DomainResult<'a>>) =
    task {
        match! result with
        | Ok result -> return OkObjectResult(result) :> IActionResult
        | Error error ->
            match error.errorData with
            | Failure ->
                let problem = problemDetails error
                problem.Status <- 500
                let result = ObjectResult(problem)
                result.StatusCode <- 500
                return result
            | Unexpected ->
                let problem = problemDetails error
                problem.Status <- 500
                let result = ObjectResult(problem)
                result.StatusCode <- 500
                return result
            | NotFound ->
                let problem = problemDetails error
                problem.Status <- 404
                return NotFoundObjectResult(problem)
            | Unauthorized ->
                let problem = problemDetails error
                problem.Status <- 401
                return UnauthorizedObjectResult(problem)
            | Validation errors ->
                let problem = validationProblemDetails error
                problem.Status <- 400

                errors
                |> Map.iter (fun key value -> problem.Errors.Add(key, value |> Array.ofList))

                return BadRequestObjectResult(problem)
            | Conflict errors ->
                let problem = validationProblemDetails error
                problem.Status <- 409

                errors
                |> Map.iter (fun key value -> problem.Errors.Add(key, value |> Array.ofList))

                return ConflictObjectResult(problem)
    }
