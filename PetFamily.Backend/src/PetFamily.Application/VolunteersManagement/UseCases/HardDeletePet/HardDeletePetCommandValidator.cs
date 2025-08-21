using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.UseCases.HardDeletePet;

public class HardDeletePetCommandValidator : AbstractValidator<HardDeletePetCommand>
{
    public HardDeletePetCommandValidator()
    {
        RuleFor(hd => hd.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(HardDeletePetCommand.VolunteerId)));

        RuleFor(hd => hd.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(HardDeletePetCommand.PetId)));
    }
}