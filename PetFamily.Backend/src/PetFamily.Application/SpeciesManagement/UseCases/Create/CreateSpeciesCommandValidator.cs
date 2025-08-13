using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.UseCases.Create;

public class CreateSpeciesCommandValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesCommandValidator()
    {
        RuleFor(x => x.Name)
            .MustBeValueObject(Name.Create);
    }
}