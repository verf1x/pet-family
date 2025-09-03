using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.MovePet;

public record MovePetCommand(Guid VolunteerId, Guid PetId, int NewPosition) : ICommand;