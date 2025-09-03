using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.Queries.GetWithPagination;

public record GetVolunteersWithPaginationQuery(int PageNumber, int PageSize) : IQuery;