namespace FsClean

type DomainError =
    { code: string
      description: string
      service: string option
      entity: string option
      operation: string option
      entityId: string option
      errorData: DomainErrorData }

and DomainErrorData =
    | Failure
    | Unexpected
    | NotFound
    | Unauthorized
    | Validation of ValidationErrorData
    | Conflict of ConflictErrorData

and ValidationErrorData = Map<string, string list>

and ConflictErrorData = Map<string, string list>

type DomainResult<'t> = Result<'t, DomainError>

[<RequireQualifiedAccess>]
module DomainError =
    let setCode code error = { error with code = code }
    let setDescription description error = { error with description = description }

    let setServiceOption service error = { error with service = service }
    let setService service = setServiceOption (Some service)
    let resetService = setServiceOption None

    let setEntityOption entity error = { error with entity = entity }
    let setEntity entity = setEntityOption (Some entity)
    let resetEntity = setEntityOption None

    let setOperationOption operation error = { error with operation = operation }
    let setOperation operation = setOperationOption (Some operation)
    let resetOperation = setOperationOption None

    let setEntityIdOption entityId error = { error with entityId = entityId }
    let setEntityId entityId = setEntityIdOption (Some entityId)
    let resetEntityId = setEntityIdOption None

    let setErrorData errorData error = { error with errorData = errorData }

    let failure =
        { code = "General.Failure"
          description = "An error occurred."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Failure }

    let unexpected =
        { failure with
            code = "General.Unexpected"
            description = "An unexpected error occurred."
            errorData = Unexpected }

    let notFound =
        { failure with
            code = "General.NotFound"
            description = $"The requested entity was not found."
            errorData = NotFound }

    let unauthorized =
        { failure with
            code = "General.Unauthorized"
            description = $"The requested operation is not authorized."
            errorData = Unauthorized }

    let validation =
        { failure with
            code = "General.Validation"
            description = $"The requested operation failed validation."
            errorData = Validation Map.empty }

    let conflict =
        { failure with
            code = "General.Conflict"
            description = $"The requested operation failed due to a conflict."
            errorData = Conflict Map.empty }
