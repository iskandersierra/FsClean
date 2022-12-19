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

module DomainError =
    let setCode code error = { error with code = code }
    let setDescription description error = { error with description = description }
    let setService service error = { error with service = service }
    let setSomeService service = setService (Some service)
    let resetService = setService None
    let setEntity entity error = { error with entity = entity }
    let setSomeEntity entity = setEntity (Some entity)
    let resetEntity = setEntity None
    let setOperation operation error = { error with operation = operation }
    let setSomeOperation operation = setOperation (Some operation)
    let resetOperation = setOperation None
    let setEntityId entityId error = { error with entityId = entityId }
    let setSomeEntityId entityId = setEntityId (Some entityId)
    let resetEntityId = setEntityId None
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

    let notFound entity entityId =
        { failure with
            code = "General.NotFound"
            description = $"The requested entity was not found."
            entity = Some entity
            entityId = Some entityId
            errorData = NotFound }

    let unauthorized entity =
        { failure with
            code = "General.Unauthorized"
            description = $"The requested operation is not authorized."
            entity = Some entity
            errorData = Unauthorized }

    let validation entity errors =
        { failure with
            code = "General.Validation"
            description = $"The requested operation failed validation."
            entity = Some entity
            errorData = Validation errors }

    let conflict entity errors =
        { failure with
            code = "General.Conflict"
            description = $"The requested operation failed due to a conflict."
            entity = Some entity
            errorData = Conflict errors }
