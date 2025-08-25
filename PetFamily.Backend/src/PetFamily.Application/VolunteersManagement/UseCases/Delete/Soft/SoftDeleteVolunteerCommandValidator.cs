using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Application.VolunteersManagement.UseCases.Delete.Hard;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.UseCases.Delete.Soft;

public class SoftDeleteVolunteerCommandValidator : AbstractValidator<SoftDeleteVolunteerCommand>
{
    public SoftDeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(SoftDeleteVolunteerCommand.VolunteerId)));
    }
}