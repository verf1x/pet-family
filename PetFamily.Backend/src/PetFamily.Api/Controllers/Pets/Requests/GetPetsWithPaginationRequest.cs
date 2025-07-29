using PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

namespace PetFamily.Api.Controllers.Pets.Requests;

public record GetPetsWithPaginationRequest(int Page, int PageSize)
{
    public GetPetsWithPaginationQuery ToQuery() => new(Page, PageSize);
}