using Dapper;
using PetFamily.Framework.Database;
using PetFamily.Framework.EntityIds;

namespace PetFamily.Framework.Extensions;

public static class SpeciesIdExtensions
{
    public static async Task<bool> IsSpeciesExistsAsync(
        this SpeciesId speciesId,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        using var connection = sqlConnectionFactory.Create();

        const string sql =
            """

                    SELECT EXISTS (
                        SELECT 1 
                        FROM species 
                        WHERE id = @id
                    )
            """;

        var parameters = new DynamicParameters();
        parameters.Add("id", speciesId.Value);

        var speciesExists = await connection.ExecuteScalarAsync<bool>(sql, parameters);

        return speciesExists;
    }
}