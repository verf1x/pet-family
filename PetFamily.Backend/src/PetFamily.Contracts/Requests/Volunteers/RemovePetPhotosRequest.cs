namespace PetFamily.Contracts.Requests.Volunteers;

public record RemovePetPhotosRequest(IEnumerable<string> PhotoPaths);