using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

public class UpdatePetHelpStatusCommandValidator : AbstractValidator<UpdatePetHelpStatusCommand>
{
    public UpdatePetHelpStatusCommandValidator()
    {
        RuleFor(u => u.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdatePetHelpStatusCommand.VolunteerId)));

        RuleFor(u => u.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdatePetHelpStatusCommand.PetId)));

        RuleFor(u => u.HelpStatus)
            .InclusiveBetween(0, 2)
            .WithError(Errors.General.ValueIsInvalid(nameof(UpdatePetHelpStatusCommand.HelpStatus)));
    }
}