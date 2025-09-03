using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Enums;
using Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class UpdatePetHelpStatusHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<int, UpdatePetHelpStatusCommand> _sut;

    public UpdatePetHelpStatusHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<int, UpdatePetHelpStatusCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdatePetHelpStatus_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        const int newHelpStatus = 2;
        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id.Value, newHelpStatus);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(newHelpStatus);
        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].HelpStatus.Should().Be((HelpStatus)newHelpStatus);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdatePetHelpStatus_WhenCommandIsInvalid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        const int newHelpStatus = -1; // Invalid status
        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id.Value, newHelpStatus);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        var updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].HelpStatus.Should().NotBe((HelpStatus)newHelpStatus);
    }
}