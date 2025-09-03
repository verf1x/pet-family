using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;

namespace Volunteers.Application.VolunteersManagement.Queries.GetWithPagination;

public class GetVolunteersWithPaginationQueryValidator
    : AbstractValidator<GetVolunteersWithPaginationQuery>
{
    public GetVolunteersWithPaginationQueryValidator()
    {
        RuleFor(vq => vq.PageSize)
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetVolunteersWithPaginationQuery.PageSize)));

        RuleFor(vq => vq.PageNumber)
            .GreaterThan(0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetVolunteersWithPaginationQuery.PageNumber)));
    }
}