using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Files;

public interface IFileProvider
{
    Task<Result<List<FilePath>, Error>> UploadPhotosAsync(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);

    Task<Result<List<string>, Error>> RemoveFilesAsync(
        IEnumerable<string> filesPaths,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> RemoveFileAsync(
        string path,
        CancellationToken cancellationToken = default);
}