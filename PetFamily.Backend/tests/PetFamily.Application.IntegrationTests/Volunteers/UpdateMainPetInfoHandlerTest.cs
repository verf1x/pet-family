using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Volunteers.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;
using Volunteers.Contracts.Dtos.Pet;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class UpdateMainPetInfoHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, UpdateMainPetInfoCommand> _sut;

    public UpdateMainPetInfoHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainPetInfoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateMainPetInfo_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var command = Fixture.BuildUpdateMainPetInfoCommand(
            volunteer.Id,
            pet.Id.Value,
            new SpeciesBreedDto(species.Id, species.Breeds[0].Id.Value));

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(pet.Id.Value);
        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        var updatedPet = updatedVolunteer.Pets[0];
        updatedPet.Id.Value.Should().Be(pet.Id.Value);
        updatedPet.Nickname.Value.Should().Be(command.Nickname);
        updatedPet.Description.Value.Should().Be(command.Description);
        updatedPet.Color.Value.Should().Be(command.Color);
        updatedPet.HealthInfo.HealthStatus.Should().Be(command.HealthInfo.HealthStatus);
        updatedPet.Address.AddressLines.Should().BeEquivalentTo(command.Address.AddressLines);
        updatedPet.Address.Locality.Should().Be(command.Address.Locality);
        updatedPet.Address.CountryCode.Should().Be(command.Address.CountryCode);
        updatedPet.Measurements.Height.Should().Be(command.Measurements.Height);
        updatedPet.Measurements.Weight.Should().Be(command.Measurements.Weight);
        updatedPet.OwnerPhoneNumber.Value.Should().Be(command.OwnerPhoneNumber);

        // TODO: добавить проверку реквизитов
    }
}