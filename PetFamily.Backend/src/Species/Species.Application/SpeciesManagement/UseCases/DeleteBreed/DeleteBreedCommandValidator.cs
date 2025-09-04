using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Species.Application.SpeciesManagement.UseCases.DeleteBreed;

public class DeleteBreedCommandValidator : AbstractValidator<DeleteBreedCommand>
{
    public DeleteBreedCommandValidator()
    {
        RuleFor(db => db.SpeciesId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(DeleteBreedCommand.SpeciesId)));

        RuleFor(db => db.BreedId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(DeleteBreedCommand.BreedId)));
    }
}