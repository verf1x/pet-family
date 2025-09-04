using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

public class UploadPetPhotosCommandValidator : AbstractValidator<UploadPetPhotosCommand>
{
    public UploadPetPhotosCommandValidator()
    {
        RuleFor(x => x.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(UploadPetPhotosCommand.VolunteerId)));

        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(UploadPetPhotosCommand.PetId)));

        RuleFor(x => x.Photos)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(UploadPetPhotosCommand.Photos)));
    }
}