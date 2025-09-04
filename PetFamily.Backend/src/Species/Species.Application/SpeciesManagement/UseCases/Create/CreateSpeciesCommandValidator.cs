using FluentValidation;
using PetFamily.Core.Validation;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Application.SpeciesManagement.UseCases.Create;

public class CreateSpeciesCommandValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesCommandValidator() =>
        RuleFor(x => x.Name)
            .MustBeValueObject(Name.Create);
}