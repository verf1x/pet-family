using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using Species.Application.SpeciesManagement.UseCases.DeleteBreed;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.IntegrationTests.Species;

public class DeleteBreedHandlerTest : SpeciesTestBase
{
    private readonly ICommandHandler<Guid, DeleteBreedCommand> _sut;

    public DeleteBreedHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteBreedCommand>>();

    [Fact]
    public async Task HandleAsync_ShouldDeleteBreed_WhenCommandIsValid()
    {
        // Arrange
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        species.AddBreeds(
            [Breed.Create("BreedToDelete").Value, Breed.Create("AnotherBreed").Value]);

        int initialBreedCount = species.Breeds.Count;

        Guid breedId = species.Breeds[0].Id.Value;
        DeleteBreedCommand command = new(species.Id, breedId);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(breedId);
        global::Species.Domain.SpeciesManagement.Species? updatedSpecies = await WriteDbContext.Species
            .FirstOrDefaultAsync();
        updatedSpecies!.Breeds.Should().HaveCount(initialBreedCount - 1);
        updatedSpecies.Breeds.Any(b => b.Id.Value == breedId).Should().BeFalse();
    }
}