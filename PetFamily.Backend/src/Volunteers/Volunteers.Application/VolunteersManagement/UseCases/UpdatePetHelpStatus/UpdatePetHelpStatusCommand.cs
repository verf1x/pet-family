using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

public record UpdatePetHelpStatusCommand(Guid VolunteerId, Guid PetId, int HelpStatus) : ICommand;