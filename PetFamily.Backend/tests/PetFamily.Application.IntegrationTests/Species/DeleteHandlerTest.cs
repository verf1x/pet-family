using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.SpeciesManagement.UseCases.Delete;
using PetFamily.TestUtils;

namespace PetFamily.Application.IntegrationTests.Species;

public class DeleteHandlerTest : SpeciesTestBase
{
    private readonly ICommandHandler<Guid, DeleteSpeciesCommand> _sut;

    public DeleteHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteSpeciesCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteSpecies_WhenCommandIsValid()
    {
        // Arrange
        var species = await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        var command = new DeleteSpeciesCommand(species.Id);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var deletedSpecies = await WriteDbContext.Species.FirstOrDefaultAsync();
        deletedSpecies.Should().BeNull();
    }
}