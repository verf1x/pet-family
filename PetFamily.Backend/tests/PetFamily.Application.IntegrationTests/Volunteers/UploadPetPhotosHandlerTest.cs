using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class UploadPetPhotosHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<List<string>, UploadPetPhotosCommand> _sut;

    public UploadPetPhotosHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<List<string>, UploadPetPhotosCommand>>();

    [Fact]
    public async Task HandleAsync_ShouldUploadPetPhotos_WhenCommandIsValid()
    {
        // Arrange
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        Pet pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);

        UploadPetPhotosCommand command = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id.Value);

        Factory.SetupFileProviderSuccessUploadMock();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        Volunteer? updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        updatedVolunteer!.Pets.Should().NotBeEmpty();
        updatedVolunteer.Pets[0].Photos.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenUploadFailed()
    {
        // Arrange
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        Pet pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);

        UploadPetPhotosCommand command = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id);

        Factory.SetupFileProviderFailedUploadMock();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}