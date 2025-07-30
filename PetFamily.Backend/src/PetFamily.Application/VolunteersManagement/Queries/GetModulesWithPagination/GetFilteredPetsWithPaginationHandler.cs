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
        var petsQuery = _readDbContext.Pets;
        
        if(!string.IsNullOrWhiteSpace(query.Nickname))
            petsQuery = petsQuery
                .Where(p => p.Nickname.Contains(query.Nickname));

        return await petsQuery.ToPagedList(
            query.PageNumber,
            query.PageSize,
            cancellationToken);
    }
} 