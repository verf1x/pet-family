using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel;

namespace Volunteers.Application.VolunteersManagement.Queries.GetPetById;

public class GetPetByIdQueryValidator : AbstractValidator<GetPetByIdQuery>
{
    public GetPetByIdQueryValidator() =>
        RuleFor(g => g.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(GetPetByIdQuery.PetId)));
}