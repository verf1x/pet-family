using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdQueryValidator : AbstractValidator<GetBreedsBySpeciesIdQuery>
{
    public GetBreedsBySpeciesIdQueryValidator()
    {
        RuleFor(gb => gb.SpeciesId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetBreedsBySpeciesIdQuery.SpeciesId)));
    }
}