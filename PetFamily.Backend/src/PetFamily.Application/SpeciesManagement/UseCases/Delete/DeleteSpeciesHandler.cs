using CSharpFunctionalExtensions;
using Dapper;
using FluentValidation;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.SpeciesManagement.UseCases.Delete;

public class DeleteSpeciesHandler : ICommandHandler<Guid, DeleteSpeciesCommand>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IValidator<DeleteSpeciesCommand> _speciesIdValidator;

    public DeleteSpeciesHandler(ISqlConnectionFactory sqlConnectionFactory, ISpeciesRepository speciesRepository,
        IValidator<DeleteSpeciesCommand> speciesIdValidator)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _speciesRepository = speciesRepository;
        _speciesIdValidator = speciesIdValidator;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _speciesIdValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var speciesId = SpeciesId.Create(command.SpeciesId);

        var hasPetsWithSpecies = await IsAnyPetsWithSpeciesAsync(speciesId);
        if (hasPetsWithSpecies)
            return Error.Failure(
                    "species.is.used.by.pets",
                    $"Cannot delete species with id {speciesId.Value}, as it is used by one or more pets.")
                .ToErrorList();
        //TODO: заменить на код 4XX
        
        var speciesResult = await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return speciesResult.Error.ToErrorList();

        var removedSpeciesId = await _speciesRepository.RemoveByIdAsync(speciesResult.Value, cancellationToken);

        return removedSpeciesId;
    }

    private async Task<bool> IsAnyPetsWithSpeciesAsync(SpeciesId speciesId)
    {
        var connection = _sqlConnectionFactory.Create();

        var anyPetsWithSpeciesQuery =
            """
            SELECT CAST(
                CASE WHEN EXISTS (
                    SELECT 1 
                    FROM pets 
                    WHERE species_id = @speciesId
                ) 
                THEN 1 
                ELSE 0 
                END 
            AS BIT)
            """;

        var parameters = new DynamicParameters();
        parameters.Add("speciesId", speciesId.Value);

        return await connection.ExecuteScalarAsync<bool>(anyPetsWithSpeciesQuery, parameters);
    }
}