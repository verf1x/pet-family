using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    public Task<Result<IReadOnlyList<FilePath>, Error>> UploadFilesAsync(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default);
}