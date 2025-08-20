using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.VolunteersManagement.ValueObjects;
using File = PetFamily.Domain.VolunteersManagement.ValueObjects.File;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetPhoto;

public class UpdateMainPetPhotoHandler : ICommandHandler<string, UpdateMainPetPhotoCommand>
{
    private readonly IValidator<UpdateMainPetPhotoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;

    public UpdateMainPetPhotoHandler(
        IValidator<UpdateMainPetPhotoCommand> validator,
        IVolunteersRepository volunteersRepository)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        UpdateMainPetPhotoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToErrorList();

        var volunteerId = VolunteerId.Create(command.VolunteerId);

        var volunteerResult = await _volunteersRepository.GetByIdAsync(
            volunteerId,
            cancellationToken);

        if (volunteerResult.IsFailure)
            return volunteerResult.Error.ToErrorList();

        var petId = PetId.Create(command.PetId);

        var petResult = volunteerResult.Value.GetPetById(petId);
        if (petResult.IsFailure)
            return petResult.Error.ToErrorList();

        var mainPhoto = new File(FilePath.Create(command.photoPath).Value);

        var mainPhotoPathResult = petResult.Value.SetMainPhoto(mainPhoto);
        if (mainPhotoPathResult.IsFailure)
            return mainPhotoPathResult.Error.ToErrorList();

        return mainPhoto.Path.Value;
    }
}