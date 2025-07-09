using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Files.Upload;

public class UploadFileHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<UploadFileHandler> _logger;

    public UploadFileHandler(IFileProvider fileProvider, ILogger<UploadFileHandler> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
    }

    public async Task<Result<string, Error>> HandleAsync(
        UploadFileRequest request,
        CancellationToken cancellationToken)
    {
        var fileData = new FileData(
            request.Stream,
            request.BucketName,
            request.ObjectName);

        var uploadFileResult = await _fileProvider.UploadFileAsync(fileData, cancellationToken);
        if(uploadFileResult.IsFailure)
        {
            _logger.LogError("An error occurred while uploading the file with name {ObjectName} to bucket {BucketName}",
                request.ObjectName, request.BucketName);
            
            return uploadFileResult.Error;
        }
        
        _logger.LogInformation("File with name {ObjectName} successfully uploaded to bucket {BucketName}",
            request.ObjectName, request.BucketName);
        
        return uploadFileResult.Value;
    }
}