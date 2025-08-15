using FluentValidation;
using PetFamily.Application.SpeciesManagement.UseCases.Delete;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.UseCases.AddBreeds;

public class AddBreedsCommandValidator : AbstractValidator<AddBreedsCommand>
{
    public AddBreedsCommandValidator()
    {
        RuleFor(ab => ab.SpeciesId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(DeleteSpeciesCommand.SpeciesId)));

        RuleForEach(ab => ab.BreedsNames)
            .NotEmpty()
            .WithError(Errors.General.ValueIsInvalid(nameof(AddBreedsCommand.BreedsNames)));
    }
}