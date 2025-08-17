using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.SoftDeletePet;

public record SoftDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;