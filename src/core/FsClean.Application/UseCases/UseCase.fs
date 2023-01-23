namespace FsClean.Application.UseCases

open FsClean.Domain

type UseCase<'command, 'result> = 'command -> AsyncDomainResult<'result>

type IUseCase<'command, 'result> =
    abstract Execute : 'command -> AsyncDomainResult<'result>

module UseCase =
    let ofInterface (useCase: IUseCase<_, _>) : UseCase<_, _> = useCase.Execute

    let toInterface (useCase: UseCase<_, _>) : IUseCase<_, _> =
        { new IUseCase<_, _> with
            member __.Execute command = useCase command }
