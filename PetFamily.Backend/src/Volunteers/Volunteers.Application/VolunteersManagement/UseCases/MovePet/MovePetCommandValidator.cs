using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.UseCases.MovePet;

public class MovePetCommandValidator : AbstractValidator<MovePetCommand>
{
    public MovePetCommandValidator()
    {
        RuleFor(mp => mp.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(MovePetCommand.VolunteerId)));

        RuleFor(mp => mp.PetId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired(nameof(MovePetCommand.PetId)));

        RuleFor(mp => mp.NewPosition)
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsInvalid(nameof(MovePetCommand.NewPosition)));
    }
}