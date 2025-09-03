using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.Framework.Models;
using PetFamily.TestUtils;
using Volunteers.Application.VolunteersManagement.Queries.GetWithPagination;
using Volunteers.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetVolunteersWithPaginationHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery> _sut;

    public GetVolunteersWithPaginationHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedListOfVolunteers_WhenQueryIsValid()
    {
        // Arrange
        var volunteer1 = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var volunteer2 = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);

        var query = new GetVolunteersWithPaginationQuery(1, 2);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().Contain(v => v.Id == volunteer1.Id);
        result.Value.Items.Should().Contain(v => v.Id == volunteer2.Id);
        result.Value.Items[0].Description.Should().Be(volunteer1.Description.Value);
        result.Value.Items[0].Experience.Should().Be(volunteer1.Experience.Value);
        result.Value.Items[0].Pets.Should().BeEmpty();
        result.Value.Items[0].PhoneNumber.Should().Be(volunteer1.PhoneNumber.Value);
        result.Value.Items[1].Description.Should().Be(volunteer2.Description.Value);
        result.Value.Items[1].Experience.Should().Be(volunteer2.Experience.Value);
        result.Value.Items[1].Pets.Should().BeEmpty();
        result.Value.Items[1].PhoneNumber.Should().Be(volunteer2.PhoneNumber.Value);
    }
}