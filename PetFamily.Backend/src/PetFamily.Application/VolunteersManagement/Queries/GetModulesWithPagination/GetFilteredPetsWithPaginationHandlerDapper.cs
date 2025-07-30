using System.Text.Json;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;
using PetFamily.Application.Models;
using PetFamily.Infrastructure;

namespace PetFamily.Application.VolunteersManagement.Queries.GetModulesWithPagination;

public class GetFilteredPetsWithPaginationHandlerDapper
    : IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFilteredPetsWithPaginationHandlerDapper(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PagedList<PetDto>> HandleAsync(
        GetFilteredPetsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();

        var totalCount = await connection.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM pets");

        const string sql = """
                           SELECT id, nickname, description, position, color, photos FROM pets
                           ORDER BY position
                           LIMIT @PageSize OFFSET @Offset
                           """;
        
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        
        var pets = await connection.QueryAsync<PetDto, string, PetDto>(
            sql,
            (pet, jsonFiles) =>
            {
                var files = JsonSerializer.Deserialize<PetPhotoDto[]>(jsonFiles) ?? [];

                pet.Photos = files;

                return pet;
            },
            splitOn: "photos",
            param: parameters);
        

        return new PagedList<PetDto>
        {
            Items = pets.ToList(),
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
            TotalCount = totalCount
        };
    }
}