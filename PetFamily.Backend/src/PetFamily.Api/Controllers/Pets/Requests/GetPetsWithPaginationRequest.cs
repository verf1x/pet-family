using PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

namespace PetFamily.Api.Controllers.Pets.Requests;

public record GetPetsWithPaginationRequest(string? Nickname, int Page, int PageSize)
{
    public GetFilteredPetsWithPaginationQuery ToQuery() => new(Nickname, Page, PageSize);
}