using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions;
using PetFamily.Application.VolunteersManagement.UseCases.Create;
using PetFamily.Application.VolunteersManagement.UseCases.Delete.Soft;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class SoftDeleteVolunteerHandlerTests : VolunteerTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _createVolunteerSut;
    private readonly ICommandHandler<Guid, SoftDeleteVolunteerCommand> _softDeleteSut;

    public SoftDeleteVolunteerHandlerTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _createVolunteerSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        _softDeleteSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, SoftDeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldSoftDeleteVolunteer_WhenCommandIsValid()
    {
        // Arrange
        var createCommand = Fixture.BuildCreateVolunteerCommand();

        // Act
        var createResult = await _createVolunteerSut.HandleAsync(createCommand, CancellationToken.None);
        var volunteerId = createResult.Value;

        var softDeleteCommand = new SoftDeleteVolunteerCommand(volunteerId);
        var softDeleteResult = await _softDeleteSut.HandleAsync(softDeleteCommand, CancellationToken.None);

        // Assert
        softDeleteResult.IsSuccess.Should().BeTrue();

        var volunteer =
            await WriteDbContext.Volunteers.FirstOrDefaultAsync(); // TODO: Обновить ReadDbContext, либо внедрить Dapper
        volunteer.Should().NotBeNull();
        volunteer.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenVolunteerDoesNotExist()
    {
        // Arrange
        var softDeleteCommand = new SoftDeleteVolunteerCommand(Guid.NewGuid());

        // Act
        var softDeleteResult = await _softDeleteSut.HandleAsync(softDeleteCommand, CancellationToken.None);

        // Assert
        softDeleteResult.IsSuccess.Should().BeFalse();
        softDeleteResult.Error.Should().NotBeNullOrEmpty();
    }
}