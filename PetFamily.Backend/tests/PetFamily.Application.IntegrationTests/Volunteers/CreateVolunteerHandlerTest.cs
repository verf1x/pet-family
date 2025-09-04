using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.Create;
using Volunteers.Contracts.Dtos;
using Volunteers.Contracts.Dtos.Volunteer;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class CreateVolunteerHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

    public CreateVolunteerHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory) =>
        _sut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();

    [Fact]
    public async Task HandleAsync_ShouldCreateVolunteer_WhenCommandIsValid()
    {
        // Arrange
        CreateVolunteerCommand command = Fixture.BuildCreateVolunteerCommand();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        Volunteer? volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        CreateVolunteerCommand command = Fixture.BuildCreateVolunteerCommand(email: "volunteer@petfamily.com");

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
        CreateVolunteerCommand command = Fixture.BuildCreateVolunteerCommand(phoneNumber: "+1234567890");

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
        CreateVolunteerCommand command = Fixture.BuildCreateVolunteerCommand(
            firstName,
            lastName,
            null,
            email,
            description,
            experienceYears,
            phoneNumber,
            new List<SocialNetworkDto> { new(socialNetworkName!, socialNetworkUrl!) },
            new List<HelpRequisiteDto> { new(helpRequisiteName!, helpRequisiteDescription!) });

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