using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.Queries.GetById;
using Volunteers.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetByIdHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<VolunteerDto, GetVolunteerByIdQuery> _sut;

    public GetByIdHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<VolunteerDto, GetVolunteerByIdQuery>>();

    [Fact]
    public async Task HandleAsync_ShouldReturnVolunteer_WhenIdIsValid()
    {
        // Arrange
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        GetVolunteerByIdQuery query = new(volunteer.Id);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(volunteer.Id);
    }
}