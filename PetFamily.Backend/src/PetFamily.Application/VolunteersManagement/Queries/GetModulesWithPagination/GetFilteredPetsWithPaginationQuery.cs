using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

public record GetFilteredPetsWithPaginationQuery(
    string? Nickname,
    int? PositionFrom,
    int? PositionTo,
    int PageNumber,
    int PageSize) : IQuery;