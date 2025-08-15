using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.Queries.GetWithPagination;

public record GetVolunteersWithPaginationQuery(int PageNumber, int PageSize) : IQuery;