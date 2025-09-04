using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;
using Species.Contracts.Dtos.Species;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.IntegrationTests.Species;

public class GetBreedsBySpeciesIdHandlerTest : SpeciesTestBase
{
    private readonly IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery> _sut;

    public GetBreedsBySpeciesIdHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery>>();

    [Fact]
    public async Task HandleAsync_ShouldReturnBreeds_WhenSpeciesExists()
    {
        // Arrange
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, WriteDbContext);

        GetBreedsBySpeciesIdQuery query = new(species.Id);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(species.Breeds.Count);

        foreach (Breed breed in species.Breeds)
        {
            result.Value.Should().Contain(b => b.Id == breed.Id.Value && b.Name == breed.Name);
        }
    }
}