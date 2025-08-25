using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class AddVolunteerHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

    public AddVolunteerHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateVolunteer_WhenCommandIsValid()
    {
        // Arrange
        var command = Fixture.BuildCreateVolunteerCommand();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await WriteDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = Fixture.BuildCreateVolunteerCommand(email: "volunteer@petfamily.com");

        // Act
        var firstTry = await _sut.HandleAsync(command, CancellationToken.None);
        var secondTry = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        firstTry.IsSuccess.Should().BeTrue();
        firstTry.Value.Should().NotBeEmpty();

        secondTry.IsSuccess.Should().BeFalse();
        secondTry.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateVolunteer_WhenPhoneNumberAlreadyExists()
    {
        // Arrange
        var command = Fixture.BuildCreateVolunteerCommand(phoneNumber: "+1234567890");

        // Act
        var firstTry = await _sut.HandleAsync(command, CancellationToken.None);
        var secondTry = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        firstTry.IsSuccess.Should().BeTrue();
        firstTry.Value.Should().NotBeEmpty();

        secondTry.IsSuccess.Should().BeFalse();
        secondTry.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("", null, null, null, null, null, null, null, null, null)]
    [InlineData(null, "", null, null, null, null, null, null, null, null)]
    [InlineData(null, null, "", null, null, null, null, null, null, null)]
    [InlineData(null, null, null, "", null, null, null, null, null, null)]
    [InlineData(null, null, null, null, -1, null, null, null, null, null)]
    [InlineData(null, null, null, null, null, "", null, null, null, null)]
    [InlineData(null, null, null, null, null, null, "", null, null, null)]
    [InlineData(null, null, null, null, null, null, null, "", null, null)]
    [InlineData(null, null, null, null, null, null, null, null, "", null)]
    [InlineData(null, null, null, null, null, null, null, null, null, "")]
    public async Task HandleAsync_ShouldReturnError_WhenCommandIsInvalid(
        string? firstName,
        string? lastName,
        string? email,
        string? description,
        int? experienceYears,
        string? phoneNumber,
        string? socialNetworkName,
        string? socialNetworkUrl,
        string? helpRequisiteName,
        string? helpRequisiteDescription)
    {
        // Arrange
        var command = Fixture.BuildCreateVolunteerCommand(
            firstName,
            lastName,
            null,
            email,
            description,
            experienceYears,
            phoneNumber,
            new List<SocialNetworkDto>() { new(socialNetworkName!, socialNetworkUrl!), },
            new List<HelpRequisiteDto>() { new(helpRequisiteName!, helpRequisiteDescription!), });

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    internal new Task InitializeAsync() => Task.CompletedTask;

    internal new async Task DisposeAsync()
    {
        Scope.Dispose();
        await Factory.ResetDatabaseAsync();
    }
}