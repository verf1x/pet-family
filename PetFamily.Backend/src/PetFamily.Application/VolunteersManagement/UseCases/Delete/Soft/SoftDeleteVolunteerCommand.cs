using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.Delete.Soft;

public record SoftDeleteVolunteerCommand(Guid VolunteerId) : ICommand;