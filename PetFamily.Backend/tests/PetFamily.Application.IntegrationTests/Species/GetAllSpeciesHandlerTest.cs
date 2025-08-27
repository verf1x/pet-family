using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.SpeciesManagement.Queries.Get;
using PetFamily.Contracts.Dtos.Species;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Species;

public class GetAllSpeciesHandlerTest : SpeciesTestBase
{
    private readonly IQueryHandler<IReadOnlyList<SpeciesDto>, GetAllSpeciesQuery> _sut;

    public GetAllSpeciesHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<IQueryHandler<IReadOnlyList<SpeciesDto>, GetAllSpeciesQuery>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllSpeciesWithBreeds()
    {
        // Arrange
        var species1 = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, WriteDbContext);
        var species2 = await SpeciesSeeder.SeedSpeciesWithBreedsAsync(SpeciesRepository, WriteDbContext);

        var query = new GetAllSpeciesQuery();

        // Act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(s => s.Id == species1.Id);
        result.Value.Should().Contain(s => s.Id == species2.Id);
        result.Value[0].Name.Should().Be(species1.Name.Value);
        result.Value[1].Name.Should().Be(species2.Name.Value);
        result.Value[0].Breeds.Should().HaveCount(species1.Breeds.Count);
        result.Value[1].Breeds.Should().HaveCount(species2.Breeds.Count);
    }
}