using CSharpFunctionalExtensions;
using PetFamily.Application.Files;
using PetFamily.Contracts.Dtos;
using PetFamily.Domain.Shared;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Extensions;

public static class AddPetCommandExtensions
{
    public static Result<List<FileData>, Error> ToDataCollection(this IEnumerable<UploadFileDto> files)
    {
        var result = new List<FileData>();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);

            var pathResult = FilePath.Create(Guid.NewGuid(), extension);
            if (pathResult.IsFailure)
                return pathResult.Error;

            var fileContent = new FileData(file.Content, pathResult.Value);
            result.Add(fileContent);
        }

        return result;
    }
}