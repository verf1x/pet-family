using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Hard;

public record HardDeleteVolunteerCommand(Guid VolunteerId) : ICommand;