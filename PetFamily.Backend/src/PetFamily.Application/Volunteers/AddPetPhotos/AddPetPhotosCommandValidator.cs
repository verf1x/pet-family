using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.AddPetPhotos;

public class AddPetPhotosCommandValidator : AbstractValidator<AddPetPhotosCommand>
{
    public AddPetPhotosCommandValidator()
    {
        RuleFor(x => x.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetPhotosCommand.VolunteerId)));
        
        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetPhotosCommand.PetId)));

        RuleFor(x => x.Photos)
            .Must(x => x.Any())
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetPhotosCommand.Photos)));
    }
}