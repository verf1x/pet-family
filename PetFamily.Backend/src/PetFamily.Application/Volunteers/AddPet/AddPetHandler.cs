using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private readonly IFileProvider _fileProvider;

    public AddPetHandler(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    // TODO: заменить FileData на Request
    public async Task<Result<string, Error>> HandleAsync(
        FileData fileData,
        CancellationToken cancellationToken = default)
    {
        return await _fileProvider.UploadFileAsync(fileData, cancellationToken);
    }
} 