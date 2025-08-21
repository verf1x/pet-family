using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.UseCases.DeleteBreed;

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