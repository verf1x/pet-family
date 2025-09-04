using CSharpFunctionalExtensions;
using PetFamily.Core.Files;
using PetFamily.SharedKernel;
using Volunteers.Contracts.Dtos;

namespace Volunteers.Application.Extensions;

public static class AddPetCommandExtensions
{
    public static Result<List<PhotoData>, Error> ToDataCollection(this IEnumerable<UploadFileDto> files)
    {
        List<PhotoData> result = new();

        foreach (UploadFileDto file in files)
        {
            string extension = Path.GetExtension(file.FileName);

            string path = $"{Guid.NewGuid()}{extension}";

            PhotoData fileContent = new(file.Content, path);
            result.Add(fileContent);
        }

        return result;
    }
}