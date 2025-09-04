using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.SoftDeletePet;

public record SoftDeletePetCommand(Guid VolunteerId, Guid PetId) : ICommand;