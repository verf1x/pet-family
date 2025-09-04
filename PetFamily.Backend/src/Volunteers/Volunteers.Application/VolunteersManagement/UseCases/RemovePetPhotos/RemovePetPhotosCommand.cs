using PetFamily.Core.Abstractions;

namespace Volunteers.Application.VolunteersManagement.UseCases.RemovePetPhotos;

public record RemovePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoPaths) : ICommand;