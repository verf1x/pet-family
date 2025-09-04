using System.Data;
using System.Text;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.Extensions;
using PetFamily.SharedKernel.Models;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Application.VolunteersManagement.Queries.GetWithPagination;

public class GetVolunteersWithPaginationHandlerDapper
    : IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetVolunteersWithPaginationHandlerDapper(ISqlConnectionFactory sqlConnectionFactory) =>
        _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<PagedList<VolunteerDto>, ErrorList>> HandleAsync(
        GetVolunteersWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        DynamicParameters parameters = new();

        long totalCount = await connection.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM volunteers");

        StringBuilder sqlQuery = new(
            "SELECT id, description, experience, phone_number as PhoneNumber FROM volunteers");

        sqlQuery.ApplyPagination(parameters, query.PageNumber, query.PageSize);

        IEnumerable<VolunteerDto> volunteers = await connection.QueryAsync<VolunteerDto>(
            sqlQuery.ToString(),
            parameters);

        return new PagedList<VolunteerDto>
        {
            Items = volunteers.ToList(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}