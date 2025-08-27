using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Models;
using PetFamily.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetFilteredPetsWithPaginationHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery> _sut;

    public GetFilteredPetsWithPaginationHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedListOfPets_WhenQueryIsValidWithNoFiltering()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, WriteDbContext);
        var pet1 = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);
        var pet2 = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[1].Id);

        var query = new GetFilteredPetsWithPaginationQuery(null, null, null, null, null, 1, 2);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().Contain(p => p.Id == pet1.Id);
        result.Value.Items.Should().Contain(p => p.Id == pet2.Id);
        result.Value.Items[0].Nickname.Should().Be(pet1.Nickname.Value);
        result.Value.Items[0].Description.Should().Be(pet1.Description.Value);
        result.Value.Items[0].Color.Should().Be(pet1.Color.Value);
        result.Value.Items[0].Photos.Should().HaveCount(pet1.Photos.Count);
        result.Value.Items[0].Position.Should().Be(pet1.Position.Value);
        result.Value.Items[1].Nickname.Should().Be(pet2.Nickname.Value);
        result.Value.Items[1].Description.Should().Be(pet2.Description.Value);
        result.Value.Items[1].Color.Should().Be(pet2.Color.Value);
        result.Value.Items[1].Photos.Should().HaveCount(pet2.Photos.Count);
        result.Value.Items[1].Position.Should().Be(pet2.Position.Value);
    }
}