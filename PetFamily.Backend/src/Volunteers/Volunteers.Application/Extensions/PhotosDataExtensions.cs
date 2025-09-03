using PetFamily.Framework.Files;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.Extensions;

public static class PhotosDataExtensions
{
    public static List<Photo> ToFilesCollection(
        this IEnumerable<PhotoData> photosData)
    {
        return photosData
            .Select(f => f.Path)
            .Select(f => new Photo(f))
            .ToList();
    }
}