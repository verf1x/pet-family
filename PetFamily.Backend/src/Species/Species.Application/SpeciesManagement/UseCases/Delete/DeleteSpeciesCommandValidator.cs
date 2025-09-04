using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Species.Application.SpeciesManagement.UseCases.Delete;

public class DeleteSpeciesCommandValidator : AbstractValidator<DeleteSpeciesCommand>
{
    public DeleteSpeciesCommandValidator() =>
        RuleFor(command => command.SpeciesId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(DeleteSpeciesCommand.SpeciesId)));
}