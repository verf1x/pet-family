using CSharpFunctionalExtensions;
using PetFamily.Application.Files;
using PetFamily.Contracts.Dtos;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Extensions;

public static class AddPetCommandExtensions
{
    public static Result<List<PhotoData>, Error> ToDataCollection(this IEnumerable<UploadFileDto> files)
    {
        var result = new List<PhotoData>();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);

            var path = $"{Guid.NewGuid()}{extension}";

            var fileContent = new PhotoData(file.Content, path);
            result.Add(fileContent);
        }

        return result;
    }
}