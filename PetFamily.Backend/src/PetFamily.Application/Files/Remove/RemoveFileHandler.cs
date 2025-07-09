using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Files.Remove;

public class RemoveFileHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<RemoveFileHandler> _logger;

    public RemoveFileHandler(IFileProvider fileProvider, ILogger<RemoveFileHandler> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
    }
    
    public async Task<Result<string, Error>> HandleAsync(
        RemoveFileRequest request,
        CancellationToken cancellationToken = default)
    {
        var removeFileResult = await _fileProvider.RemoveFileAsync(
            request.BucketName, 
            request.ObjectName, 
            cancellationToken);
        if (removeFileResult.IsFailure)
        {
            _logger.LogError("An error occurred while removing the file with name {ObjectName}", request.ObjectName);
            return removeFileResult.Error;
        }
        
        _logger.LogInformation("File with name {ObjectName} removed successfully", request.ObjectName);
        return removeFileResult.Value;
    }
}