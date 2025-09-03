using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.HardDeletePet;

public record HardDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;