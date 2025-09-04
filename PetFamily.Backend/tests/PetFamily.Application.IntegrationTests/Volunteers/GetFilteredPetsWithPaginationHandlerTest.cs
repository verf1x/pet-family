using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Models;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;
using Volunteers.Contracts.Dtos.Pet;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetFilteredPetsWithPaginationHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery> _sut;

    public GetFilteredPetsWithPaginationHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery>>();

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedListOfPets_WhenQueryIsValidWithNoFiltering()
    {
        // Arrange
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Pet pet1 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);
        Pet pet2 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[1].Id);

        GetFilteredPetsWithPaginationQuery query = new(null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, 1, 10);

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

    [Fact]
    public async Task HandleAsync_ShouldReturnFilteredPets_WhenQueryIsValidWithFiltering()
    {
        // Arrange
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Pet pet1 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);
        Pet pet2 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[1].Id);

        GetFilteredPetsWithPaginationQuery query = new(
            [volunteer.Id.Value],
            pet2.Nickname.Value,
            null,
            pet2.SpeciesBreed.SpeciesId,
            pet2.SpeciesBreed.BreedId,
            pet2.Color.Value,
            pet2.Address.CountryCode,
            pet2.Address.Locality,
            pet2.Measurements.Height,
            pet2.Measurements.Weight,
            (int)pet2.HelpStatus,
            null,
            null,
            null,
            null,
            1,
            10);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.Should().Contain(p => p.Id == pet2.Id);
        result.Value.Items[0].VolunteerId.Should().Be(volunteer.Id);
        result.Value.Items[0].Nickname.Should().Be(pet2.Nickname.Value);
        result.Value.Items[0].SpeciesBreed.SpeciesId.Should().Be(pet2.SpeciesBreed.SpeciesId);
        result.Value.Items[0].SpeciesBreed.BreedId.Should().Be(pet2.SpeciesBreed.BreedId);
        result.Value.Items[0].Color.Should().Be(pet2.Color.Value);
        result.Value.Items[0].Address.CountryCode.Should().Be(pet2.Address.CountryCode);
        result.Value.Items[0].Address.Locality.Should().Be(pet2.Address.Locality);
        result.Value.Items[0].Measurements.Height.Should().Be(pet2.Measurements.Height);
        result.Value.Items[0].Measurements.Weight.Should().Be(pet2.Measurements.Weight);
        result.Value.Items[0].HelpStatus.Should().Be((int)pet2.HelpStatus);
    }
}