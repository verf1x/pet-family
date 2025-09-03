using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Framework.Abstractions;
using Volunteers.Application.VolunteersManagement.UseCases.Create;
using Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class HardDeleteVolunteerHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _createVolunteerSut;
    private readonly ICommandHandler<Guid, HardDeleteVolunteerCommand> _hardDeleteSut;

    public HardDeleteVolunteerHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _createVolunteerSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        _hardDeleteSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, HardDeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldHardDeleteVolunteer_WhenCommandIsValid()
    {
        // Arrange
        var createCommand = Fixture.BuildCreateVolunteerCommand();

        // Act
        var createResult = await _createVolunteerSut.HandleAsync(createCommand, CancellationToken.None);
        var volunteerId = createResult.Value;

        var hardDeleteCommand = new HardDeleteVolunteerCommand(volunteerId);
        var hardDeleteResult = await _hardDeleteSut.HandleAsync(hardDeleteCommand, CancellationToken.None);

        // Assert
        hardDeleteResult.IsSuccess.Should().BeTrue();

        var volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer.Should().BeNull();
    }
}