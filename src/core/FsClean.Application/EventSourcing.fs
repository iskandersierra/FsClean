namespace FsClean.Application.EventSourcing

open System.Threading
open System.Threading.Tasks

type EventEnvelope<'event> =
    { eventId: string
      meta: Map<string, string>
      event: 'event }

type PersistedEventEnvelope<'event> =
    { partitionId: string
      entityType: string
      entityId: string
      globalSequence: uint64
      partitionSequence: uint64
      entitySequence: uint64
      eventId: string
      meta: Map<string, string>
      event: 'event }

type EventStoreAppend<'event> = CancellationToken -> EventStoreAppendParams<'event> -> Task<EventStoreAppendResult>

and EventStoreAppendParams<'event> =
    { partitionId: string
      entityType: string
      entityId: string
      entitySequence: uint64
      events: EventEnvelope<'event> seq }

and EventStoreAppendResult =
    { globalSequence: uint64
      partitionSequence: uint64
      entitySequence: uint64 }

type EventStoreRead<'event> = CancellationToken -> EventStoreReadParams -> Task<EventStoreReadResult<'event>>

and EventStoreReadParams = { filter: EventStoreReadFilter }

and EventStoreReadFilter =
    | EventStoreReadNoFilter
    | EventStoreReadPartition of EventStoreReadPartition
    | EventStoreReadEntityType of EventStoreReadEntityType
    | EventStoreReadEntityInstance of EventStoreReadEntityInstance

and EventStoreReadPartition =
    { partitionId: string
      sequence: uint64 option }

and EventStoreReadEntityType =
    { partitionId: string
      entityType: string
      sequence: uint64 option }

and EventStoreReadEntityInstance =
    { partitionId: string
      entityType: string
      entityId: string
      sequence: uint64 option }

and EventStoreReadResult<'event> =
    { events: PersistedEventEnvelope<'event>
      sequence: uint64
      isLastPage: bool }

type EventStore<'event> =
    { append: EventStoreAppend<'event>
      read: EventStoreRead<'event> }

type IEventStore<'event> =
    abstract AppendAsync :
        parameters: EventStoreAppendParams<'event> * cancellationToken: CancellationToken ->
        Task<EventStoreAppendResult>

    abstract ReadAsync :
        parameters: EventStoreReadParams * cancellationToken: CancellationToken -> Task<EventStoreReadResult<'event>>

module EventStore =
    let toInterface (store: EventStore<'event>) =
        { new IEventStore<'event> with
            member __.AppendAsync(parameters, cancellationToken) =
                store.append cancellationToken parameters

            member __.ReadAsync(parameters, cancellationToken) = store.read cancellationToken parameters }

    let ofInterface (store: IEventStore<'event>) =
        { append = (fun cancellationToken parameters -> store.AppendAsync(parameters, cancellationToken))
          read = (fun cancellationToken parameters -> store.ReadAsync(parameters, cancellationToken)) }
