using System.Text;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;
using PetFamily.Contracts.Dtos.Volunteer;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.Queries.GetWithPagination;

public class GetVolunteersWithPaginationHandlerDapper
    : IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetVolunteersWithPaginationHandlerDapper(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PagedList<VolunteerDto>, ErrorList>> HandleAsync(
        GetVolunteersWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();

        var totalCount = await connection.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM volunteers");

        var sqlQuery = new StringBuilder(
            "SELECT id, description, experience, phone_number FROM volunteers");

        sqlQuery.ApplyPagination(parameters, query.PageNumber, query.PageSize);

        var volunteers = await connection.QueryAsync<VolunteerDto>(
            sqlQuery.ToString(),
            param: parameters);

        return new PagedList<VolunteerDto>
        {
            Items = volunteers.ToList(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
        };
    }
}