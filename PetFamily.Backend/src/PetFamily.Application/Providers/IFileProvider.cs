using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    Task<UnitResult<Error>> UploadFilesAsync(
        FilesData filesData,
        CancellationToken cancellationToken = default);
}