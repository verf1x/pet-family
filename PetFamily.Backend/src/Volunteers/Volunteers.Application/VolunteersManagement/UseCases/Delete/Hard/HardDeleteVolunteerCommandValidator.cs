using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public class HardDeleteVolunteerCommandValidator : AbstractValidator<HardDeleteVolunteerCommand>
{
    public HardDeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(HardDeleteVolunteerCommand.VolunteerId)));
    }
}