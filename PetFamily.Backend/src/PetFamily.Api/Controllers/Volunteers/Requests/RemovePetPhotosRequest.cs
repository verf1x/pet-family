using PetFamily.Application.Volunteers.RemovePetPhotos;

namespace PetFamily.Api.Controllers.Volunteers.Requests;

public record RemovePetPhotosRequest(IEnumerable<string> PhotoNames)
{
    public RemovePetPhotosCommand ToCommand(Guid volunteerId, Guid petId)
        => new RemovePetPhotosCommand(volunteerId, petId, PhotoNames);
}