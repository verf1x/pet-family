using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Species.Application.SpeciesManagement.UseCases.AddBreeds;

namespace PetFamily.Application.IntegrationTests.Species;

public class AddBreedsHandlerTest : SpeciesTestBase
{
    private readonly ICommandHandler<List<Guid>, AddBreedsCommand> _sut;

    public AddBreedsHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<List<Guid>, AddBreedsCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldAddBreedsToSpecies_WhenCommandIsValid()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);

        var newBreeds = new List<string> { "Breed1", "Breed2" };
        var command = new AddBreedsCommand(species.Id, newBreeds);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(newBreeds.Count);
        var updatedSpecies = await WriteDbContext.Species
            .FirstOrDefaultAsync(s => s.Id == species.Id);
        updatedSpecies!.Breeds.Should().HaveCount(newBreeds.Count);
        updatedSpecies.Breeds.Select(b => b.Name).Should().Contain(newBreeds);
    }
}