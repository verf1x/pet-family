using System.Data;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;
using Species.Contracts.Dtos.Species;

namespace Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public class GetBreedsBySpeciesIdHandler : IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetBreedsBySpeciesIdHandler(ISqlConnectionFactory sqlConnectionFactory) =>
        _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<IReadOnlyList<BreedDto>, ErrorList>> HandleAsync(
        GetBreedsBySpeciesIdQuery query,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        SpeciesId speciesId = SpeciesId.Create(query.SpeciesId);

        const string sqlQuery = "SELECT breeds FROM species WHERE id = @speciesId";

        DynamicParameters parameters = new();
        parameters.Add("speciesId", speciesId.Value);

        string? breedsJson = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery, parameters);

        if (string.IsNullOrEmpty(breedsJson))
        {
            return Errors.General.NotFound(query.SpeciesId).ToErrorList();
        }

        List<BreedDto>? breeds = JsonSerializer.Deserialize<List<BreedDto>>(breedsJson, JsonSerializerOptions.Default);

        return breeds ?? [];
    }
}