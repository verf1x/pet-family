using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdQueryValidator : AbstractValidator<GetBreedsBySpeciesIdQuery>
{
    public GetBreedsBySpeciesIdQueryValidator() =>
        RuleFor(gb => gb.SpeciesId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetBreedsBySpeciesIdQuery.SpeciesId)));
}