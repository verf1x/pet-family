using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    Task<UnitResult<Error>> UploadFilesAsync(
        FilesData filesData,
        CancellationToken cancellationToken = default);
    
    Task<Result<string, Error>> RemoveFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);
    
    Task<Result<string, Error>> GetPresignedUrlAsync(
        GetPresignedUrlData presignedUrlData,
        CancellationToken cancellationToken = default);
}