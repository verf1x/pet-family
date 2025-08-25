using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.Volunteer;
using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class AddVolunteerHandlerTests : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    private readonly IntegrationTestsWebFactory _factory;
    private readonly Fixture _fixture;
    private readonly WriteDbContext _writeDbContext;
    private readonly IServiceScope _scope;
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

    public AddVolunteerHandlerTests(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateVolunteer_WhenCommandIsValid()
    {
        // Arrange
        var command = _fixture.BuildCreateVolunteerCommand();

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var volunteer = await _writeDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = _fixture.BuildCreateVolunteerCommand(email: "volunteer@petfamily.com");

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
        var command = _fixture.BuildCreateVolunteerCommand(phoneNumber: "+1234567890");

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
        var command = _fixture.BuildCreateVolunteerCommand(
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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await _factory.ResetDatabaseAsync();
    }
}