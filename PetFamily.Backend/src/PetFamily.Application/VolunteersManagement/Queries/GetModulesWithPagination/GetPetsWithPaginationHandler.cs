using PetFamily.Application.Database;
using PetFamily.Application.Dtos;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;

namespace PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

public class GetPetsWithPaginationHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetPetsWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
    
    public async Task<PagedList<PetDto>> HandleAsync(
        GetPetsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var petsQuery = _readDbContext.Pets.AsQueryable();

        return await petsQuery.ToPagedList(
            query.PageNumber,
            query.PageSize,
            cancellationToken);
    }
}