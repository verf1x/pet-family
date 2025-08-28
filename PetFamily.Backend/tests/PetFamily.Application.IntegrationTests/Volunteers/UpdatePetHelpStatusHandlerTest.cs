using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;
using PetFamily.Domain.VolunteersManagement.Enums;
using PetFamily.TestUtils;

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
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        const int newHelpStatus = 2;
        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id.Value, newHelpStatus);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(newHelpStatus);
        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].HelpStatus.Should().Be((HelpStatus)newHelpStatus);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdatePetHelpStatus_WhenCommandIsInvalid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        const int newHelpStatus = -1; // Invalid status
        var command = new UpdatePetHelpStatusCommand(volunteer.Id, pet.Id.Value, newHelpStatus);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].HelpStatus.Should().NotBe((HelpStatus)newHelpStatus);
    }
}