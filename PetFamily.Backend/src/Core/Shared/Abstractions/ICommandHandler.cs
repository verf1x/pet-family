using CSharpFunctionalExtensions;

namespace PetFamily.Framework.Abstractions;

public interface ICommandHandler<TResponse, in TCommand>
    where TCommand : ICommand
{
    Task<Result<TResponse, ErrorList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<ErrorList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}