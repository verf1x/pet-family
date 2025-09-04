using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public record HardDeleteVolunteerCommand(Guid VolunteerId) : ICommand;