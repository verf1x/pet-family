using System.Data;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using Species.Contracts.Dtos.Species;

namespace Species.Application.SpeciesManagement.Queries.Get;

public class GetAllSpeciesHandler : IQueryHandler<IReadOnlyList<SpeciesDto>, GetAllSpeciesQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetAllSpeciesHandler(ISqlConnectionFactory sqlConnectionFactory) =>
        _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<IReadOnlyList<SpeciesDto>, ErrorList>> HandleAsync(
        GetAllSpeciesQuery query,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        string sqlQuery = "SELECT id, name, breeds FROM species";

        IEnumerable<SpeciesDto> species = await connection.QueryAsync<SpeciesDto, string, SpeciesDto>(
            sqlQuery,
            (speciesDto, breedsJson) =>
            {
                speciesDto.Breeds = !string.IsNullOrWhiteSpace(breedsJson)
                    ? JsonSerializer.Deserialize<BreedDto[]>(breedsJson)!
                    : [];

                return speciesDto;
            },
            splitOn: "breeds");

        return species.ToList();
    }
}