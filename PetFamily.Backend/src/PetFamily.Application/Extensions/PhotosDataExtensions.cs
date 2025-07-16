using PetFamily.Application.FileProvider;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Extensions;

public static class PhotosDataExtensions
{
    public static List<Photo> ToPhotosCollection(this IEnumerable<AddPhotoData> photosData)
    {
        return photosData
            .Select(f => f.PhotoPath)
            .Select(f => new Photo(f))
            .ToList();
    }
}