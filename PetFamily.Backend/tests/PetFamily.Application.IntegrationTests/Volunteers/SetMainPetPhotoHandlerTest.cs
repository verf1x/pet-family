using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Volunteers.Application.VolunteersManagement.UseCases.SetMainPetPhoto;
using Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class SetMainPetPhotoHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<List<string>, UploadPetPhotosCommand> _uploadPhotosSut;
    private readonly ICommandHandler<string, SetMainPetPhotoCommand> _setMainPhotoSut;

    public SetMainPetPhotoHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _uploadPhotosSut =
            Scope.ServiceProvider.GetRequiredService<ICommandHandler<List<string>, UploadPetPhotosCommand>>();
        _setMainPhotoSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<string, SetMainPetPhotoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldSetMainPetPhoto_WhenCommandIsValid()
    {
        // Arrange
        var volunteer = await VolunteerSeeder.SeedVolunteerAsync(VolunteersRepository, VolunteersWriteDbContext);
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, SpeciesWriteDbContext);
        var pet = await VolunteerSeeder.SeedPetAsync(VolunteersWriteDbContext, volunteer, species.Id, species.Breeds[0].Id);

        var uploadCommand = Fixture.BuildUploadPetPhotosCommand(volunteer.Id, pet.Id);

        // Act
        // TODO: Придумать чето с транзакциями и пофиксить
        var uploadResult = await _uploadPhotosSut.HandleAsync(uploadCommand, CancellationToken.None);

        var setMainPhotoCommand = new SetMainPetPhotoCommand(volunteer.Id, pet.Id, uploadResult.Value[1]);

        var setMainPhotoResult = await _setMainPhotoSut.HandleAsync(setMainPhotoCommand, CancellationToken.None);

        var uploadedPhotos = uploadResult.Value;

        // Assert
        uploadResult.IsSuccess.Should().BeTrue();
        setMainPhotoResult.IsSuccess.Should().BeTrue();
        setMainPhotoResult.Value.Should().Be(uploadedPhotos[1]);
        pet.MainPhoto.Should().NotBeNull();
        pet.MainPhoto.Path.Should().Be(uploadedPhotos[1]);
        pet.Photos.Should().HaveCount(2);
        pet.Photos[0].Path.Should().Be(uploadedPhotos[0]);
        pet.Photos[1].Path.Should().Be(uploadedPhotos[1]);
    }
}