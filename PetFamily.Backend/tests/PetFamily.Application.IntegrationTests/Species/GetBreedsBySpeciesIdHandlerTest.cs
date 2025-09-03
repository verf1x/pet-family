using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using PetFamily.TestUtils;
using Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;
using Species.Contracts.Dtos.Species;

namespace PetFamily.Application.IntegrationTests.Species;

public class GetBreedsBySpeciesIdHandlerTest : SpeciesTestBase
{
    private readonly IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery> _sut;

    public GetBreedsBySpeciesIdHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<IReadOnlyList<BreedDto>, GetBreedsBySpeciesIdQuery>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBreeds_WhenSpeciesExists()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, WriteDbContext);

        var query = new GetBreedsBySpeciesIdQuery(species.Id);

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(species.Breeds.Count);

        foreach (var breed in species.Breeds)
            result.Value.Should().Contain(b => b.Id == breed.Id.Value && b.Name == breed.Name);
    }
}