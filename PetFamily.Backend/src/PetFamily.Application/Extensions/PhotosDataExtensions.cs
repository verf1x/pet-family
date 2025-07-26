using PetFamily.Application.FileProvider;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Extensions;

public static class PhotosDataExtensions
{
    public static List<Photo> ToPhotosCollection(this IEnumerable<PhotoData> photosData)
    {
        return photosData
            .Select(f => f.Path)
            .Select(f => new Photo(f))
            .ToList();
    }
}