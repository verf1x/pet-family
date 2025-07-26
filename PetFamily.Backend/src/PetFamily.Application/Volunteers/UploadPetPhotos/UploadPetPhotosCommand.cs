using PetFamily.Application.Dtos;

namespace PetFamily.Application.Volunteers.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<UploadFileDto> Photos);