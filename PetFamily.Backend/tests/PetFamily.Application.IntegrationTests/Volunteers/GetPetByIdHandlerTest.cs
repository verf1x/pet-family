using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.Queries.GetPetById;
using Volunteers.Contracts.Dtos.Pet;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class GetPetByIdHandlerTest : VolunteerTestBase
{
    private readonly IQueryHandler<PetDto, GetPetByIdQuery> _sut;

    public GetPetByIdHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<PetDto, GetPetByIdQuery>>();

    [Fact]
    public async Task HandleAsync_ShouldReturnPet_WhenQueryIsValid()
    {
        // Arrange
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Pet pet1 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);
        Pet pet2 = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[1].Id);

        GetPetByIdQuery query = new(pet2.Id);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(pet2.Id);
        result.Value.VolunteerId.Should().Be(volunteer.Id);
        result.Value.Nickname.Should().Be(pet2.Nickname.Value);
        result.Value.SpeciesBreed.SpeciesId.Should().Be(pet2.SpeciesBreed.SpeciesId);
        result.Value.SpeciesBreed.BreedId.Should().Be(pet2.SpeciesBreed.BreedId);
        result.Value.Color.Should().Be(pet2.Color.Value);
        result.Value.Address.CountryCode.Should().Be(pet2.Address.CountryCode);
        result.Value.Address.Locality.Should().Be(pet2.Address.Locality);
        result.Value.Measurements.Height.Should().Be(pet2.Measurements.Height);
        result.Value.Measurements.Weight.Should().Be(pet2.Measurements.Weight);
        result.Value.HelpStatus.Should().Be((int)pet2.HelpStatus);
    }
}