namespace FsClean.Application.EventSourcing

open System.Threading
open System.Threading.Tasks

type PartitionId = string
type EntityType = string
type EntityIdentifier = string
type EventIdentifier = string
type EventSequence = string

type EventMetadata = Map<string, string>

type EventEnvelope<'event> =
    { eventId: EventIdentifier
      meta: EventMetadata
      event: 'event }

type PersistedEventEnvelope<'event> =
    { partitionId: PartitionId
      entityType: EntityType
      entityId: EntityIdentifier
      eventId: EventIdentifier
      globalSequence: EventSequence
      instanceSequence: EventSequence
      meta: EventMetadata
      event: 'event }

type EventStoreAppend<'event> = CancellationToken -> EventStoreAppendParams<'event> -> Task<EventStoreAppendResult>

and EventStoreAppendParams<'event> =
    { partitionId: PartitionId
      entityType: EntityType
      entityId: EntityIdentifier
      instanceSequence: EventSequence
      event: EventEnvelope<'event> }

and EventStoreAppendResult =
    { globalSequence: EventSequence
      instanceSequence: EventSequence }

type EventStoreReader<'event> =
    { read: CancellationToken -> Task<EventStoreReadResult<'event>>
      dispose: unit -> Task }

and EventStoreReadResult<'event> =
    { events: PersistedEventEnvelope<'event>[]
      isLastPage: bool }

type EventStoreGetReader<'event> = CancellationToken -> EventStoreGetReader -> Task<EventStoreReader<'event>>

and EventStoreGetReader =
    { subject: EventStoreReadSubject
      events: EventStoreReadEvents
      batchSize: int option }

and EventStoreReadSubject =
    | EventStoreReadAllSubjects
    | EventStoreReadSubjects of EventStoreReadSubjects

and EventStoreReadSubjects =
    { partitionId: PartitionId option
      entityType: EntityType option
      entityId: EntityIdentifier option }

and EventStoreReadEvents =
    | EventStoreReadAllEvents
    | EventStoreReadAfterGlobalSequence of EventSequence
    //| EventStoreReadAfterInstanceSequence of EventSequence

[<RequireQualifiedAccess>]
module EventStoreGetReader =
    let empty =
        { subject = EventStoreReadAllSubjects
          events = EventStoreReadAllEvents
          batchSize = None }

type EventStore<'event> =
    { append: EventStoreAppend<'event>
      getReader: EventStoreGetReader<'event> }

// type IEventStore<'event> =
//     abstract AppendAsync :
//         parameters: EventStoreAppendParams<'event> * cancellationToken: CancellationToken ->
//         Task<EventStoreAppendResult>

//     abstract ReadAsync :
//         parameters: EventStoreReadParams * cancellationToken: CancellationToken -> Task<EventStoreReadResult<'event>>

module EventStore =
    // let toInterface (store: EventStore<'event>) =
    //     { new IEventStore<'event> with
    //         member __.AppendAsync(parameters, cancellationToken) =
    //             store.append cancellationToken parameters

    //         member __.ReadAsync(parameters, cancellationToken) = store.read cancellationToken parameters }

    // let ofInterface (store: IEventStore<'event>) =
    //     { append = (fun cancellationToken parameters -> store.AppendAsync(parameters, cancellationToken))
    //       read = (fun cancellationToken parameters -> store.ReadAsync(parameters, cancellationToken)) }
    ()
