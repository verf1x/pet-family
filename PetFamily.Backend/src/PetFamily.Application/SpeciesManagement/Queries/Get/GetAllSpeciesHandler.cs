using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Contracts.Dtos.Species;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.Queries.Get;

public class GetAllSpeciesHandler : IQueryHandler<IReadOnlyList<SpeciesDto>, GetAllSpeciesQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetAllSpeciesHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }
    
    public async Task<Result<IReadOnlyList<SpeciesDto>, ErrorList>> HandleAsync(GetAllSpeciesQuery query, CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();
        
        var sqlQuery = "SELECT id, name, species_breeds FROM species";
        
        var species = await connection.QueryAsync<SpeciesDto>(sqlQuery);
        
        return species.ToList();
    }
}