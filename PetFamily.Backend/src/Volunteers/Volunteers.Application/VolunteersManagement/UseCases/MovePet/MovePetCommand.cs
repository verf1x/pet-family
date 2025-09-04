using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.MovePet;

public record MovePetCommand(Guid VolunteerId, Guid PetId, int NewPosition) : ICommand;