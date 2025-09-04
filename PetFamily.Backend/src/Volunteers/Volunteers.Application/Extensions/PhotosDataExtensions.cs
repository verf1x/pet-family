using PetFamily.Core.Files;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.Extensions;

public static class PhotosDataExtensions
{
    public static List<Photo> ToFilesCollection(
        this IEnumerable<PhotoData> photosData) =>
        photosData
            .Select(f => f.Path)
            .Select(f => new Photo(f))
            .ToList();
}