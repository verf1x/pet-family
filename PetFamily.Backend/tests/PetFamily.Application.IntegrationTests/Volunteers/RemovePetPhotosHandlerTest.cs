using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.RemovePetPhotos;
using Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class RemovePetPhotosHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<List<string>, RemovePetPhotosCommand> _removeSut;
    private readonly ICommandHandler<List<string>, UploadPetPhotosCommand> _uploadSut;

    public RemovePetPhotosHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _uploadSut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<List<string>, UploadPetPhotosCommand>>();
        _removeSut = Scope.ServiceProvider
            .GetRequiredService<ICommandHandler<List<string>, RemovePetPhotosCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldRemovePetPhotos_WhenCommandIsValid()
    {
        // Arrange
        Volunteer volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        Pet pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id,
            species.Breeds[0].Id);

        UploadPetPhotosCommand uploadCommand = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id.Value);
        var uploadResult = await _uploadSut.HandleAsync(uploadCommand, CancellationToken.None);

        RemovePetPhotosCommand removeCommand = new(volunteer.Id, pet.Id.Value, uploadResult.Value);

        // Act
        var removeResult = await _removeSut.HandleAsync(removeCommand, CancellationToken.None);

        // Assert
        removeResult.IsSuccess.Should().BeTrue();
        Volunteer? updatedVolunteer = await VolunteersWriteDbContext.Volunteers.FindAsync(volunteer.Id);
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].Photos.Should().BeEmpty();

        // TODO: пофиксить проблему с транзакциями
    }
}