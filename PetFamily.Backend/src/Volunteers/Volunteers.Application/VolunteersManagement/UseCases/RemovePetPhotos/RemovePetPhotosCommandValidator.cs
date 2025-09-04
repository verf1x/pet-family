using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.UseCases.RemovePetPhotos;

public class RemovePetPhotosCommandValidator : AbstractValidator<RemovePetPhotosCommand>
{
    public RemovePetPhotosCommandValidator()
    {
        RuleFor(x => x.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(RemovePetPhotosCommand.VolunteerId)));

        RuleFor(x => x.PetId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(RemovePetPhotosCommand.PetId)));

        RuleFor(x => x.PhotoPaths)
            .Must(x => x.Any())
            .WithError(Errors.General.ValueIsRequired(nameof(RemovePetPhotosCommand.PhotoPaths)));
    }
}