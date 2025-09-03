using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Volunteers.Application.VolunteersManagement.UseCases.SoftDeletePet;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class SoftDeletePetHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, SoftDeletePetCommand> _sut;

    public SoftDeletePetHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, SoftDeletePetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldSoftDeletePet_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);
        var command = new SoftDeletePetCommand(volunteer.Id, pet.Id);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(pet.Id);
        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].IsDeleted.Should().BeTrue();
    }
}