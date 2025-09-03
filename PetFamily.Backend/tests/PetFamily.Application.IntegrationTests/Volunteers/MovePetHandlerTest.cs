using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Volunteers.Application.VolunteersManagement.UseCases.MovePet;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class MovePetHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<int, MovePetCommand> _sut;

    public MovePetHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<int, MovePetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldMovePetToNewPosition_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var petToMove = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);
        await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[1].Id);

        var command = new MovePetCommand(volunteer.Id, petToMove.Id.Value, 2);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(2);
        updatedVolunteer.Pets[0].Position.Value.Should().Be(2);
        updatedVolunteer.Pets[1].Position.Value.Should().Be(1);
    }
}