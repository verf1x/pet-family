using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.Queries.GetPetById;

public class GetPetByIdQueryValidator : AbstractValidator<GetPetByIdQuery>
{
    public GetPetByIdQueryValidator()
    {
        RuleFor(g => g.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(GetPetByIdQuery.PetId)));
    }
}