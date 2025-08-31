using Dapper;
using PetFamily.Application.Database;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Application.Extensions;

public static class BreedIdExtensions
{
    public static async Task<bool> IsBreedExistsAsync(
        this BreedId breedId,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql =
            """

                    SELECT EXISTS (
                        SELECT 1
                        FROM species, jsonb_array_elements(breeds) as breed
                        WHERE (breed ->> 'Id')::uuid = @breedId
                    )
            """;

        var parameters = new DynamicParameters();
        parameters.Add("breedId", breedId.Value);

        var breedExists = await connection.ExecuteScalarAsync<bool>(sql, parameters);

        return breedExists;
    }
}