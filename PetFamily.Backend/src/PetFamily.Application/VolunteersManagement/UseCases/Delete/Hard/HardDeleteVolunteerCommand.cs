using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.Delete.Hard;

public record HardDeleteVolunteerCommand(Guid VolunteerId) : ICommand;