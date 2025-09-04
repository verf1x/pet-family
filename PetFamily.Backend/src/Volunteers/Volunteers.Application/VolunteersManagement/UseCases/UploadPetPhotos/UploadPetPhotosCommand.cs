using PetFamily.Core.Abstractions;
using Volunteers.Contracts.Dtos;

namespace Volunteers.Application.VolunteersManagement.UseCases.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<UploadFileDto> Photos) : ICommand;