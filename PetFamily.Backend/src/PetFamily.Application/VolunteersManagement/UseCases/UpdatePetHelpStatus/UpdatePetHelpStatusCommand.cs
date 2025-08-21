using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdatePetHelpStatus;

public record UpdatePetHelpStatusCommand(Guid VolunteerId, Guid PetId, int HelpStatus) : ICommand;