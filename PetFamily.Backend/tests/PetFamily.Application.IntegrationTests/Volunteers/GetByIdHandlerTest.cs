using Dapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.Queries.GetById;
using PetFamily.Contracts.Dtos.Volunteer;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetByIdHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> _sut;

    public GetByIdHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnVolunteer_WhenIdIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var query = new GetVolunteerByIdQuery(volunteer.Id);

        var connection = SqlConnectionFactory.Create();
        const string sqlQuery = "SELECT COUNT(*) FROM Volunteers WHERE Id = @Id";
        var parameters = new DynamicParameters();
        parameters.Add("Id", volunteer.Id.Value);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(volunteer.Id);
        int volunteersCount = await connection.ExecuteScalarAsync<int>(sqlQuery, parameters);
        volunteersCount.Should().Be(1);
    }
}