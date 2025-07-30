using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

public record GetFilteredPetsWithPaginationQuery(string? Nickname, int PageNumber, int PageSize) : IQuery;