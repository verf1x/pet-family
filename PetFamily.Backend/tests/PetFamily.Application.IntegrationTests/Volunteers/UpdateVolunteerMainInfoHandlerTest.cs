using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Volunteers.Domain.VolunteersManagement.Entities;
using Volunteers.Application.VolunteersManagement.UseCases.Create;
using Volunteers.Application.VolunteersManagement.UseCases.UpdateMainInfo;

namespace PetFamily.Application.IntegrationTests.Volunteers;

public class UpdateVolunteerMainInfoHandlerTest : VolunteerTestBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _createSut;
    private readonly ICommandHandler<Guid, UpdateMainInfoCommand> _updateMainInfoSut;

    public UpdateVolunteerMainInfoHandlerTest(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _createSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
        _updateMainInfoSut = Scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateVolunteerMainInfo_WhenCommandIsValid()
    {
        // Arrange
        CreateVolunteerCommand createCommand = Fixture.BuildCreateVolunteerCommand();
        var volunteerId = await _createSut.HandleAsync(createCommand, CancellationToken.None);

        UpdateMainInfoCommand updateCommand = Fixture.BuildUpdateVolunteerMainInfoCommand(volunteerId.Value);

        // Act
        var result = await _updateMainInfoSut.HandleAsync(updateCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(volunteerId.Value);

        Volunteer? volunteer = await VolunteersWriteDbContext.Volunteers.FirstOrDefaultAsync();
        volunteer.Should().NotBeNull();

        volunteer.FullName.FirstName.Should().Be(updateCommand.FullName.FirstName);
        volunteer.FullName.LastName.Should().Be(updateCommand.FullName.LastName);
        volunteer.FullName.MiddleName.Should().Be(updateCommand.FullName.MiddleName);
        volunteer.Email.Value.Should().Be(updateCommand.Email);
        volunteer.Description.Value.Should().Be(updateCommand.Description);
        volunteer.Experience.Value.Should().Be(updateCommand.ExperienceYears);
        volunteer.PhoneNumber.Value.Should().Be(updateCommand.PhoneNumber);
    }
}