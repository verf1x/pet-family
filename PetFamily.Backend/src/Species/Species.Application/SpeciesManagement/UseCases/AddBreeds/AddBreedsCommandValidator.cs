using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;
using Species.Application.SpeciesManagement.UseCases.Delete;

namespace Species.Application.SpeciesManagement.UseCases.AddBreeds;

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