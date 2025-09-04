using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.HardDeletePet;

public record HardDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;