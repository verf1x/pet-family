using CSharpFunctionalExtensions;

namespace PetFamily.Framework.Files;

public interface IFileProvider
{
    Task<Result<List<string>, Error>> UploadPhotosAsync(
        IEnumerable<PhotoData> filesData,
        CancellationToken cancellationToken = default);

    Task<Result<List<string>, Error>> RemoveFilesAsync(
        IEnumerable<string> filesPaths,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> RemoveFileAsync(
        string path,
        CancellationToken cancellationToken = default);
}