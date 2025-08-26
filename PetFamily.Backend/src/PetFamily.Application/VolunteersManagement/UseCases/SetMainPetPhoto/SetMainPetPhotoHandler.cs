using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.VolunteersManagement.UseCases.SetMainPetPhoto;

public class SetMainPetPhotoHandler : ICommandHandler<string, SetMainPetPhotoCommand>
{
    private readonly IValidator<SetMainPetPhotoCommand> _validator;
    private readonly IVolunteersRepository _volunteersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetMainPetPhotoHandler(
        IValidator<SetMainPetPhotoCommand> validator,
        IVolunteersRepository volunteersRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _volunteersRepository = volunteersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        SetMainPetPhotoCommand command,
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

        var mainPhoto = new Photo(command.PhotoPath);

        var mainPhotoPathResult = petResult.Value.SetMainPhoto(mainPhoto);
        if (mainPhotoPathResult.IsFailure)
            return mainPhotoPathResult.Error.ToErrorList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mainPhoto.Path;
    }
}