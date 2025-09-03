using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using Species.Application.SpeciesManagement.UseCases.Create;

namespace PetFamily.Application.IntegrationTests.Species;

public class CreateSpeciesHandlerTest : SpeciesTestBase
{
    private readonly ICommandHandler<Guid, CreateSpeciesCommand> _sut;

    public CreateSpeciesHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateSpeciesCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateSpeciesWithBreeds_WhenCommandIsValid()
    {
        // Arrange
        var command = Fixture.BuildCreateSpeciesCommand();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenSpeciesWithNameAlreadyExists()
    {
        // Arrange
        var command1 = Fixture.BuildCreateSpeciesCommand("Cat");
        var command2 = Fixture.BuildCreateSpeciesCommand("Cat");

        // Act
        var result1 = await _sut.HandleAsync(command1, CancellationToken.None);
        var result2 = await _sut.HandleAsync(command2, CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result1.Value.Should().NotBeEmpty();
        result2.IsSuccess.Should().BeFalse();
        result2.Error.Should().NotBeNullOrEmpty();
    }
}