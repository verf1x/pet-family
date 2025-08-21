using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.HardDeletePet;

public record HardDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;