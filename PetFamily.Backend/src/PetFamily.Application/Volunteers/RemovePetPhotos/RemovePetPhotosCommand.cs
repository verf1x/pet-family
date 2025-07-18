namespace PetFamily.Application.Volunteers.RemovePetPhotos;

public record RemovePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoNames);