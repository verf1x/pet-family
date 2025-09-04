using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.TestUtils;
using Species.Application.SpeciesManagement.UseCases.Delete;

namespace PetFamily.Application.IntegrationTests.Species;

public class DeleteHandlerTest : SpeciesTestBase
{
    private readonly ICommandHandler<Guid, DeleteSpeciesCommand> _sut;

    public DeleteHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteSpeciesCommand>>();

    [Fact]
    public async Task HandleAsync_ShouldDeleteSpecies_WhenCommandIsValid()
    {
        // Arrange
        global::Species.Domain.SpeciesManagement.Species species =
            await SpeciesSeeder.SeedSpeciesAsync(SpeciesRepository, WriteDbContext);
        DeleteSpeciesCommand command = new(species.Id);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        global::Species.Domain.SpeciesManagement.Species? deletedSpecies =
            await WriteDbContext.Species.FirstOrDefaultAsync();
        deletedSpecies.Should().BeNull();
    }
}