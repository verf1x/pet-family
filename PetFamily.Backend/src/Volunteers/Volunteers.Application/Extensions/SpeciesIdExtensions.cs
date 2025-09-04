using Dapper;
using PetFamily.Core.Database;
using PetFamily.SharedKernel.EntityIds;

namespace Volunteers.Application.Extensions;

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

        DynamicParameters parameters = new();
        parameters.Add("id", speciesId.Value);

        var speciesExists = await connection.ExecuteScalarAsync<bool>(sql, parameters);

        return speciesExists;
    }
}