using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Soft;

public class SoftDeleteVolunteerCommandValidator : AbstractValidator<SoftDeleteVolunteerCommand>
{
    public SoftDeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(SoftDeleteVolunteerCommand.VolunteerId)));
    }
}