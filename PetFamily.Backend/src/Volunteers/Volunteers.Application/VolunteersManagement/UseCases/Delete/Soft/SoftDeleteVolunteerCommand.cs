using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.Delete.Soft;

public record SoftDeleteVolunteerCommand(Guid VolunteerId) : ICommand;