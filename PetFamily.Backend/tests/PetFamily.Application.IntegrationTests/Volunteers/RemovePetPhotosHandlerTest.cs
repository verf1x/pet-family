using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.RemovePetPhotos;
using PetFamily.Application.VolunteersManagement.UseCases.UploadPetPhotos;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class RemovePetPhotosHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<List<string>, UploadPetPhotosCommand> _uploadSut;
    private readonly ICommandHandler<List<string>, RemovePetPhotosCommand> _removeSut;

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
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, WriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(WriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var uploadCommand = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id.Value);
        var uploadResult = await _uploadSut.HandleAsync(uploadCommand, CancellationToken.None);

        var removeCommand = new RemovePetPhotosCommand(volunteer.Id, pet.Id.Value, uploadResult.Value);

        // Act
        var removeResult = await _removeSut.HandleAsync(removeCommand, CancellationToken.None);

        // Assert
        removeResult.IsSuccess.Should().BeTrue();
        var updatedVolunteer = await WriteDbContext.Volunteers.FindAsync(volunteer.Id);
        updatedVolunteer!.Pets.Should().HaveCount(1);
        updatedVolunteer.Pets[0].Photos.Should().BeEmpty();

        // TODO: пофиксить проблему с транзакциями
    }
}