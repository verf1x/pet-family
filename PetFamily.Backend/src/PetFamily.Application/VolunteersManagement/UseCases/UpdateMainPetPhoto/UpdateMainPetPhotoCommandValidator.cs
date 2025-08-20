using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetPhoto;

public class UpdateMainPetPhotoCommandValidator : AbstractValidator<UpdateMainPetPhotoCommand>
{
    public UpdateMainPetPhotoCommandValidator()
    {
        RuleFor(x => x.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(UpdateMainPetPhotoCommand.VolunteerId)));

        RuleFor(x => x.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(UpdateMainPetPhotoCommand.PetId)));

        RuleFor(x => x.photoPath)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateMainPetPhotoCommand.photoPath)));
    }
}