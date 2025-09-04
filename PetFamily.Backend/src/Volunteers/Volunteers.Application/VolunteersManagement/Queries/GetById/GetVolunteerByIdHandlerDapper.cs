using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Database;
using PetFamily.SharedKernel;
using Volunteers.Contracts.Dtos.Volunteer;

namespace Volunteers.Application.VolunteersManagement.Queries.GetById;

public class GetVolunteerByIdHandlerDapper : IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetVolunteerByIdHandlerDapper(ISqlConnectionFactory sqlConnectionFactory) =>
        _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result<VolunteerDto, ErrorList>> HandleAsync(
        GetVolunteerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _sqlConnectionFactory.Create();

        const string sqlQuery =
            """
            SELECT id, description, experience, phone_number from volunteers
            WHERE id = @VolunteerId AND is_deleted = false
            """;

        DynamicParameters parameters = new();
        parameters.Add("VolunteerId", query.VolunteerId);

        VolunteerDto? volunteerDto = await connection.QuerySingleOrDefaultAsync<VolunteerDto>(
            sqlQuery,
            parameters);

        if (volunteerDto is null)
        {
            return Errors.General.NotFound(query.VolunteerId).ToErrorList();
        }

        return volunteerDto;
    }
}