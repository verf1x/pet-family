using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Dtos;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;

namespace PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

public class GetFilteredPetsWithPaginationHandler 
    : IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery>
{
    private readonly IReadDbContext _readDbContext;

    public GetFilteredPetsWithPaginationHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
    
    public async Task<PagedList<PetDto>> HandleAsync(
        GetFilteredPetsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var petsQuery = _readDbContext.Pets.AsQueryable();
        
        petsQuery = petsQuery.WhereIf(
            !string.IsNullOrEmpty(query.Nickname),
            p => p.Nickname.Contains(query.Nickname!));

        petsQuery = petsQuery
            .WhereIf(query.PositionTo is not null,
                p => p.Position <= query.PositionTo!.Value);
            
        petsQuery = petsQuery
            .WhereIf(query.PositionFrom is not null,
                p => p.Position >= query.PositionFrom!.Value);

        return await petsQuery
            .OrderBy(p => p.Position)
            .ToPagedList(query.PageNumber, query.PageSize, cancellationToken);
    }
} 