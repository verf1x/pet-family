using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class UploadPetPhotosHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<List<string>, UploadPetPhotosCommand> _sut;

    public UploadPetPhotosHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<List<string>, UploadPetPhotosCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUploadPetPhotos_WhenCommandIsValid()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteerRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var command = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id.Value);

        Factory.SetupFileProviderSuccessUploadMock();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        var updatedVolunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().NotBeEmpty();
        updatedVolunteer.Pets[0].Photos.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenUploadFailed()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteerRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var command = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id);

        Factory.SetupFileProviderFailedUploadMock();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}