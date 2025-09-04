using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.Create;
using Volunteers.Application.VolunteersManagement.UseCases.Delete.Soft;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class SoftDeleteVolunteerHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _createVolunteerSut;
    private readonly ICommandHandler<Guid, SoftDeleteVolunteerCommand> _softDeleteSut;

    public SoftDeleteVolunteerHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _createVolunteerSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        _softDeleteSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, SoftDeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldSoftDeleteVolunteer_WhenCommandIsValid()
    {
        // Arrange
        CreateVolunteerCommand createCommand = Fixture.BuildCreateVolunteerCommand();

        // Act
        var createResult = await _createVolunteerSut.HandleAsync(createCommand, CancellationToken.None);
        var volunteerId = createResult.Value;

        SoftDeleteVolunteerCommand softDeleteCommand = new(volunteerId);
        var softDeleteResult = await _softDeleteSut.HandleAsync(softDeleteCommand, CancellationToken.None);

        // Assert
        softDeleteResult.IsSuccess.Should().BeTrue();

        Volunteer? volunteer =
            await VolunteersWriteDbContext.Volunteers
                .FirstOrDefaultAsync(); // TODO: Обновить ReadDbContext, либо внедрить Dapper
        volunteer.Should().NotBeNull();
        volunteer.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenVolunteerDoesNotExist()
    {
        // Arrange
        SoftDeleteVolunteerCommand softDeleteCommand = new(Guid.NewGuid());

        // Act
        var softDeleteResult = await _softDeleteSut.HandleAsync(softDeleteCommand, CancellationToken.None);

        // Assert
        softDeleteResult.IsSuccess.Should().BeFalse();
        softDeleteResult.Error.Should().NotBeNullOrEmpty();
    }
}