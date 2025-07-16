using PetFamily.Application.Dtos;

namespace PetFamily.Application.Volunteers.AddPetPhotos;

public record AddPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<CreateFileDto> Photos);