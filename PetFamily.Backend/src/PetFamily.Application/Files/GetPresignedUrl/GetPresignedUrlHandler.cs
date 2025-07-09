using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.Files.GetPresignedUrl;

public class GetPresignedUrlHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<GetPresignedUrlHandler> _logger;

    public GetPresignedUrlHandler(IFileProvider fileProvider, ILogger<GetPresignedUrlHandler> logger)
    {
        _fileProvider = fileProvider;
        _logger = logger;
    }
    
    public async Task<Result<string, Error>> HandleAsync(
        GetPresignedUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        var presignedUrlData = new GetPresignedUrlData(
            request.BucketName,
            request.ObjectName,
            request.ExpirationTime);
        
        var presignedUrlResult = await _fileProvider.GetPresignedUrlAsync(
            presignedUrlData,
            cancellationToken);
        
        if (presignedUrlResult.IsFailure)
        {
            _logger.LogError("An error occurred while getting the presigned URL for file {ObjectName} in bucket {BucketName}",
                request.ObjectName, request.BucketName);
            return presignedUrlResult.Error;
        }
        
        _logger.LogInformation("Presigned URL for file {ObjectName} in bucket {BucketName} retrieved successfully",
            request.ObjectName, request.BucketName);
        
        return presignedUrlResult.Value;
    }
}