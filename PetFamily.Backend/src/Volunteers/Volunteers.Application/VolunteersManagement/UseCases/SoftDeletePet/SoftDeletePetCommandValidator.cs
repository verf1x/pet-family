using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.UseCases.SoftDeletePet;

public class SoftDeletePetCommandValidator : AbstractValidator<SoftDeletePetCommand>
{
    public SoftDeletePetCommandValidator()
    {
        RuleFor(sd => sd.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(SoftDeletePetCommand.VolunteerId)));

        RuleFor(sd => sd.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(SoftDeletePetCommand.PetId)));
    }
}