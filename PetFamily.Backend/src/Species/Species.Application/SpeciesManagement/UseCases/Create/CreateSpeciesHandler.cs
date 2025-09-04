using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement.UseCases.Create;

public class CreateSpeciesHandler : ICommandHandler<Guid, CreateSpeciesCommand>
{
    private readonly ILogger<CreateSpeciesHandler> _logger;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IValidator<CreateSpeciesCommand> _validator;

    public CreateSpeciesHandler(
        ISpeciesRepository speciesRepository,
        IValidator<CreateSpeciesCommand> validator,
        ILogger<CreateSpeciesHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> HandleAsync(
        CreateSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        Name? name = Name.Create(command.Name).Value;

        Result<Domain.SpeciesManagement.Species> speciesByNameResult = await _speciesRepository
            .GetByNameAsync(name, cancellationToken);

        if (speciesByNameResult.IsSuccess)
        {
            return Errors.Species.AlreadyExists().ToErrorList();
        }

        SpeciesId id = SpeciesId.CreateNew();

        Domain.SpeciesManagement.Species species = new(id, name);

        Guid result = await _speciesRepository.AddAsync(species, cancellationToken);

        _logger.LogInformation("Created species with ID: {id}", id);

        return result;
    }
}