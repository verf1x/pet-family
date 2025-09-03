using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.Database;
using PetFamily.Framework.EntityIds;
using PetFamily.Framework.Extensions;
using Species.Application.Extensions;

namespace Species.Application.SpeciesManagement.UseCases.AddBreeds;

public class AddBreedsHandler : ICommandHandler<List<Guid>, AddBreedsCommand>
{
    private readonly IValidator<AddBreedsCommand> _validator;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;

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
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var speciesId = SpeciesId.Create(command.SpeciesId);

        var speciesResult = await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return speciesResult.Error.ToErrorList();

        var breeds = command.BreedsNames.ToBreedsCollection();

        speciesResult.Value.AddBreeds(breeds);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breeds
            .Select(b => b.Id.Value)
            .ToList();
    }
}