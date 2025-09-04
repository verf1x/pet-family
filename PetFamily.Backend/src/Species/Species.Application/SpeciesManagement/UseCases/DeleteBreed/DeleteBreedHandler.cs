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

namespace Species.Application.SpeciesManagement.UseCases.DeleteBreed;

public class DeleteBreedHandler : ICommandHandler<Guid, DeleteBreedCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteBreedCommand> _validator;

    public DeleteBreedHandler(
        IValidator<DeleteBreedCommand> validator,
        ISpeciesRepository speciesRepository,
        ISqlConnectionFactory sqlConnectionFactory,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _speciesRepository = speciesRepository;
        _sqlConnectionFactory = sqlConnectionFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        DeleteBreedCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        BreedId breedId = BreedId.Create(command.BreedId);

        bool hasPetsWithBreed = await IsAnyPetsWithBreedAsync(breedId);
        if (hasPetsWithBreed)
        {
            return Error.Validation(
                    "breed.is.used.by.pets",
                    $"Cannot delete breed with id {breedId.Value}, as it is used by one or more pets.",
                    nameof(command.BreedId))
                .ToErrorList();
        }

        SpeciesId speciesId = SpeciesId.Create(command.SpeciesId);

        Result<Domain.SpeciesManagement.Species, Error> speciesResult =
            await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
        {
            return speciesResult.Error.ToErrorList();
        }

        speciesResult.Value.RemoveBreeds([breedId]);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breedId.Value;
    }

    private async Task<bool> IsAnyPetsWithBreedAsync(BreedId breedId)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        string anyPetsWithBreedQuery =
            """
            SELECT CAST(
                CASE WHEN EXISTS (
                    SELECT 1 
                    FROM pets
                    WHERE breed_id = @breedId
                ) 
                THEN 1
                ELSE 0
                END
            AS BIT)
            """;

        DynamicParameters parameters = new();
        parameters.Add("breedId", breedId.Value);

        return await connection.ExecuteScalarAsync<bool>(anyPetsWithBreedQuery, parameters);
    }
}