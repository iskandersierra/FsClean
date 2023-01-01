namespace FsClean.Application

open FsClean.Domain

type GenerateId = unit -> string

type IdGenerator = { generate: GenerateId }

module IdGenerator =
    let entityIdGenerator = { generate = EntityId.newGuid }
