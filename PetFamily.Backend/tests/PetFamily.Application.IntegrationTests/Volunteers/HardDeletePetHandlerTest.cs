using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.HardDeletePet;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class HardDeletePetHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, HardDeletePetCommand> _sut;

    public HardDeletePetHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, HardDeletePetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldHardDeletePet_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var command = new HardDeletePetCommand(volunteer.Id, pet.Id.Value);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(pet.Id.Value);
        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().BeEmpty();
    }
}