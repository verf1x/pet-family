using FluentValidation;
using PetFamily.Framework.Validation;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement.UseCases.Create;

public class CreateSpeciesCommandValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesCommandValidator()
    {
        RuleFor(x => x.Name)
            .MustBeValueObject(Name.Create);
    }
}