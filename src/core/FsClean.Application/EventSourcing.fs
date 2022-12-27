namespace FsClean.Application.EventSourcing

open System.Threading
open System.Threading.Tasks
open FSharp.Control

type EventEnvelope =
    { eventId: string
      meta: Map<string, string>
      data: byte []
      schema: string }

type PersistedEventEnvelope =
    { partitionId: string
      entityType: string
      entityId: string
      eventId: string
      version: uint64
      meta: Map<string, string>
      data: byte []
      schema: string }

type EventStoreAppend = CancellationToken -> EventStoreAppendParams -> Task<EventStoreAppendResult>

and EventStoreAppendParams =
    { partitionId: string
      entityType: string
      entityId: string
      currentVersion: uint64
      events: EventEnvelope seq }

and EventStoreAppendResult = { currentVersion: uint64 }

type EventStoreRead = CancellationToken -> EventStoreReadParams -> AsyncSeq<EventStoreReadPage>

and EventStoreReadParams =
    { filter: EventStoreReadFilter
      version: EventStoreReadVersion }
// TODO: Add subscription parameters

and EventStoreReadFilter =
    | EventStoreReadNoFilter
    | EventStoreReadPartition of partitionId: string
    | EventStoreReadEntityType of partitionId: string * entityType: string
    | EventStoreReadEntity of partitionId: string * entityType: string * entityId: string

and EventStoreReadVersion =
    | EventStoreReadAllEvents
    | EventStoreReadAfterVersion of version: uint64

and EventStoreReadPage =
    { events: PersistedEventEnvelope
      currentVersion: uint64 }

type EventStore =
    { append: EventStoreAppend
      read: EventStoreRead }

type IEventStore =
    abstract AppendAsync :
        parameters: EventStoreAppendParams * cancellationToken: CancellationToken -> Task<EventStoreAppendResult>

    abstract ReadAsync :
        parameters: EventStoreReadParams * cancellationToken: CancellationToken -> AsyncSeq<EventStoreReadPage>

module EventStore =
    let toInterface (store: EventStore) =
        { new IEventStore with
            member __.AppendAsync(parameters, cancellationToken) =
                store.append cancellationToken parameters

            member __.ReadAsync(parameters, cancellationToken) = store.read cancellationToken parameters }

    let ofInterface (store: IEventStore) =
        { append = (fun cancellationToken parameters -> store.AppendAsync(parameters, cancellationToken))
          read = (fun cancellationToken parameters -> store.ReadAsync(parameters, cancellationToken)) }
