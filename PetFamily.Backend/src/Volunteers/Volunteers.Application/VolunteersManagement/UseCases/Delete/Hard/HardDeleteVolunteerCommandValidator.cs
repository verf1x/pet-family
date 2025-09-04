using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public class HardDeleteVolunteerCommandValidator : AbstractValidator<HardDeleteVolunteerCommand>
{
    public HardDeleteVolunteerCommandValidator() =>
        RuleFor(r => r.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(HardDeleteVolunteerCommand.VolunteerId)));
}