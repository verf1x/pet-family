using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Contracts.Dtos.Species;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdHandler : IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetBreedsBySpeciesIdHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<BreedDto>, ErrorList>> HandleAsync(
        GetBreedsBySpeciesIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();

        var speciesId = SpeciesId.Create(query.SpeciesId);

        const string sqlQuery = "SELECT breeds FROM species WHERE id = @speciesId";

        var parameters = new DynamicParameters();
        parameters.Add("speciesId", speciesId.Value);

        var breedsJson = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery, parameters);

        if (string.IsNullOrEmpty(breedsJson))
            return Errors.General.NotFound(query.SpeciesId).ToErrorList();

        var breeds = JsonSerializer.Deserialize<List<BreedDto>>(breedsJson, JsonSerializerOptions.Default);

        return breeds ?? [];
    }
}