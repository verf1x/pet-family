using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.AddPet;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class AddPetHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, AddPetCommand> _sut;

    public AddPetHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, AddPetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddPetToVolunteer_WhenCommandIsValid()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteerRepository, WriteDbContext);
        var command =
            Fixture.BuildAddPetCommand(volunteer.Id, new SpeciesBreedDto(species.Id, species.Breeds[0].Id.Value));

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].Id.Value.Should().Be(result.Value);
    }
}