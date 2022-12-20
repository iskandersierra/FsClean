module FsClean.DomainErrorTests

open System
open Xunit
open Swensen.Unquote
open FsCheck.Xunit

open FsClean

[<Fact>]
let ``failure MUST have proper values`` () =
    let error = DomainError.failure

    let expected =
        { code = "General.Failure"
          description = "An error occurred."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Failure }

    test <@ error = expected @>

[<Fact>]
let ``unexpected MUST have proper values`` () =
    let error = DomainError.unexpected

    let expected =
        { code = "General.Unexpected"
          description = "An unexpected error occurred."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Unexpected }

    test <@ error = expected @>


[<Fact>]
let ``notFound MUST have proper values`` () =
    let error = DomainError.notFound

    let expected =
        { code = "General.NotFound"
          description = "The requested entity was not found."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = NotFound }

    test <@ error = expected @>

[<Fact>]
let ``unauthorized MUST have proper values`` () =
    let error = DomainError.unauthorized

    let expected =
        { code = "General.Unauthorized"
          description = "The requested operation is not authorized."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Unauthorized }

    test <@ error = expected @>

[<Fact>]
let ``validation MUST have proper values`` () =
    let error = DomainError.validation

    let expected =
        { code = "General.Validation"
          description = "The requested operation failed validation."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Validation Map.empty }

    test <@ error = expected @>

[<Fact>]
let ``conflict MUST have proper values`` () =
    let error = DomainError.conflict

    let expected =
        { code = "General.Conflict"
          description = "The requested operation failed due to a conflict."
          service = None
          entity = None
          operation = None
          entityId = None
          errorData = Conflict Map.empty }

    test <@ error = expected @>

[<Property>]
let ``setCode MUST set the code`` code error =
    let expected = { error with code = code }
    let actual = error |> DomainError.setCode code
    test <@ actual = expected @>

[<Property>]
let ``setDescription MUST set the description`` description error =
    let expected = { error with description = description }
    let actual = error |> DomainError.setDescription description
    test <@ actual = expected @>

[<Property>]
let ``setServiceOption MUST set the service`` service error =
    let expected = { error with service = service }
    let actual = error |> DomainError.setServiceOption service
    test <@ actual = expected @>

[<Property>]
let ``setService MUST set the service`` service error =
    let expected = { error with service = Some service }
    let actual = error |> DomainError.setService service
    test <@ actual = expected @>

[<Property>]
let ``resetService MUST set the service to None`` error =
    let expected = { error with service = None }
    let actual = error |> DomainError.resetService
    test <@ actual = expected @>

[<Property>]
let ``setEntityOption MUST set the entity`` entity error =
    let expected = { error with entity = entity }
    let actual = error |> DomainError.setEntityOption entity
    test <@ actual = expected @>

[<Property>]
let ``setEntity MUST set the entity`` entity error =
    let expected = { error with entity = Some entity }
    let actual = error |> DomainError.setEntity entity
    test <@ actual = expected @>

[<Property>]
let ``resetEntity MUST set the entity to None`` error =
    let expected = { error with entity = None }
    let actual = error |> DomainError.resetEntity
    test <@ actual = expected @>

[<Property>]
let ``setOperationOption MUST set the operation`` operation error =
    let expected = { error with operation = operation }
    let actual = error |> DomainError.setOperationOption operation
    test <@ actual = expected @>

[<Property>]
let ``setOperation MUST set the operation`` operation error =
    let expected = { error with operation = Some operation }
    let actual = error |> DomainError.setOperation operation
    test <@ actual = expected @>

[<Property>]
let ``resetOperation MUST set the operation to None`` error =
    let expected = { error with operation = None }
    let actual = error |> DomainError.resetOperation
    test <@ actual = expected @>

[<Property>]
let ``setEntityIdOption MUST set the entityId`` entityId error =
    let expected = { error with entityId = entityId }
    let actual = error |> DomainError.setEntityIdOption entityId
    test <@ actual = expected @>

[<Property>]
let ``setEntityId MUST set the entityId`` entityId error =
    let expected = { error with entityId = Some entityId }
    let actual = error |> DomainError.setEntityId entityId
    test <@ actual = expected @>

[<Property>]
let ``resetEntityId MUST set the entityId to None`` error =
    let expected = { error with entityId = None }
    let actual = error |> DomainError.resetEntityId
    test <@ actual = expected @>

[<Property>]
let ``setErrorData MUST set the errorData`` errorData error =
    let expected = { error with errorData = errorData }
    let actual = error |> DomainError.setErrorData errorData
    test <@ actual = expected @>
