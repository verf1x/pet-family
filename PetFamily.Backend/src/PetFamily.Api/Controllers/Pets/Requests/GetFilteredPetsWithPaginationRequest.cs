using PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

namespace PetFamily.Api.Controllers.Pets.Requests;

public record GetFilteredPetsWithPaginationRequest(
    string? Nickname,
    int? PositionFrom,
    int? PositionTo,
    int Page,
    int PageSize)
{
    public GetFilteredPetsWithPaginationQuery ToQuery() 
        => new(
            Nickname,
            PositionFrom,
            PositionTo,
            Page,
            PageSize);
}