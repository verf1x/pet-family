using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using PetFamily.SharedKernel.Extensions;
using Species.Application.Extensions;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement.UseCases.AddBreeds;

public class AddBreedsHandler : ICommandHandler<List<Guid>, AddBreedsCommand>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddBreedsCommand> _validator;

    public AddBreedsHandler(
        IValidator<AddBreedsCommand> validator,
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<Guid>, ErrorList>> HandleAsync(
        AddBreedsCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult? validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        SpeciesId speciesId = SpeciesId.Create(command.SpeciesId);

        Result<Domain.SpeciesManagement.Species, Error> speciesResult =
            await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
        {
            return speciesResult.Error.ToErrorList();
        }

        List<Breed> breeds = command.BreedsNames.ToBreedsCollection();

        speciesResult.Value.AddBreeds(breeds);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breeds
            .Select(b => b.Id.Value)
            .ToList();
    }
}