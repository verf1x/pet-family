using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.UseCases.SoftDeletePet;

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