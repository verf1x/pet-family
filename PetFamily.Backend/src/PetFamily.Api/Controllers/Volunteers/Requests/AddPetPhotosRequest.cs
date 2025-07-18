using PetFamily.Application.Dtos;
using PetFamily.Application.Volunteers.AddPetPhotos;

namespace PetFamily.Api.Controllers.Volunteers.Requests;

public record AddPetPhotosRequest(IFormFileCollection Photos)
{
    public AddPetPhotosCommand ToCommand(Guid volunteerId, Guid petId, List<CreateFileDto> photos)
    {
        return new AddPetPhotosCommand(volunteerId, petId, photos);
    }
}