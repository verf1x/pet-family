using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.UseCases.HardDeletePet;

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