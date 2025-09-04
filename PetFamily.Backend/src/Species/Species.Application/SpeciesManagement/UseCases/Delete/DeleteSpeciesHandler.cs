using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;

namespace Species.Application.SpeciesManagement.UseCases.Delete;

public class DeleteSpeciesHandler : ICommandHandler<Guid, DeleteSpeciesCommand>
{
    private readonly IValidator<DeleteSpeciesCommand> _speciesIdValidator;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public DeleteSpeciesHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        ISpeciesRepository speciesRepository,
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
        ValidationResult? validationResult = await _speciesIdValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpeciesId speciesId = SpeciesId.Create(command.SpeciesId);

        bool hasPetsWithSpecies = await IsAnyPetsWithSpeciesAsync(speciesId);
        if (hasPetsWithSpecies)
        {
            return Error.Validation(
                    "species.is.used.by.pets",
                    $"Cannot delete species with id {speciesId.Value}, as it is used by one or more pets.",
                    nameof(command.SpeciesId))
                .ToErrorList();
        }

        Result<Domain.SpeciesManagement.Species, Error> speciesResult =
            await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
        {
            return speciesResult.Error.ToErrorList();
        }

        Guid removedSpeciesId = await _speciesRepository.RemoveAsync(speciesResult.Value, cancellationToken);

        return removedSpeciesId;
    }

    private async Task<bool> IsAnyPetsWithSpeciesAsync(SpeciesId speciesId)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        string anyPetsWithSpeciesQuery =
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

        DynamicParameters parameters = new();
        parameters.Add("speciesId", speciesId.Value);

        return await connection.ExecuteScalarAsync<bool>(anyPetsWithSpeciesQuery, parameters);
    }
}