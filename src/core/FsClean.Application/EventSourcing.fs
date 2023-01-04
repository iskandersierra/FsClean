namespace FsClean.Application.EventSourcing

open System.Threading
open System.Threading.Tasks

open FsClean

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
    { events: PersistedEventEnvelope<'event> []
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

module EventStore =
    let mapAppend toInnerEvent (append: EventStoreAppend<'innerEvent>) : EventStoreAppend<'outterEvent> =
        fun ct outterParams ->
            let innerParams =
                { partitionId = outterParams.partitionId
                  entityType = outterParams.entityType
                  entityId = outterParams.entityId
                  instanceSequence = outterParams.instanceSequence
                  event =
                    { eventId = outterParams.event.eventId
                      meta = outterParams.event.meta
                      event = toInnerEvent outterParams.event.event } }

            append ct innerParams

    let mapGetReader toOutterEvent (getReader: EventStoreGetReader<'innerEvent>) : EventStoreGetReader<'outterEvent> =
        fun ct outterParams ->
            task {
                let innerParams =
                    { subject = outterParams.subject
                      events = outterParams.events
                      batchSize = outterParams.batchSize }

                let! innerReader = getReader ct innerParams

                return
                    { read =
                        fun ct ->
                            task {
                                let! innerResult = innerReader.read ct

                                let events =
                                    innerResult.events
                                    |> Array.map (fun innerEvent ->
                                        { partitionId = innerEvent.partitionId
                                          entityType = innerEvent.entityType
                                          entityId = innerEvent.entityId
                                          eventId = innerEvent.eventId
                                          globalSequence = innerEvent.globalSequence
                                          instanceSequence = innerEvent.instanceSequence
                                          meta = innerEvent.meta
                                          event = toOutterEvent innerEvent.event })

                                return
                                    { events = events
                                      isLastPage = innerResult.isLastPage }
                            }
                      dispose = innerReader.dispose }
            }
