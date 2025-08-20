using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.UseCases.SetMainPetPhoto;

public class SetMainPetPhotoCommandValidator : AbstractValidator<SetMainPetPhotoCommand>
{
    public SetMainPetPhotoCommandValidator()
    {
        RuleFor(x => x.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(SetMainPetPhotoCommand.VolunteerId)));

        RuleFor(x => x.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(SetMainPetPhotoCommand.PetId)));

        RuleFor(x => x.PhotoPath)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(SetMainPetPhotoCommand.PhotoPath)));
    }
}