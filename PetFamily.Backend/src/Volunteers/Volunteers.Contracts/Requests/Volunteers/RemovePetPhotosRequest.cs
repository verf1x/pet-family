namespace Volunteers.Contracts.Requests.Volunteers;

public record RemovePetPhotosRequest(IEnumerable<string> PhotoPaths);