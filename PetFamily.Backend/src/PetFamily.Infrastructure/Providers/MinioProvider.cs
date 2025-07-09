using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.Providers;

public class MinioProvider : IFileProvider
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<string, Error>> UploadFileAsync(
        FileData fileData, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(fileData.BucketName);
        
            bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
            if (!bucketExists)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(fileData.BucketName);
            
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
        
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(fileData.BucketName)
                .WithStreamData(fileData.Stream)
                .WithObjectSize(fileData.Stream.Length)
                .WithObject(fileData.ObjectName);
        
            var result = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            
            return result.ObjectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the file to Minio");
            return Error.Failure("file.upload", "An error occurred while uploading the file to Minio");
        }
    }

    public async Task<Result<string, Error>> RemoveFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            
            bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
            
            if (!bucketExists)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
            
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            
            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
            
            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while removing the file from Minio");
            return Error.Failure("file.delete", "An error occurred while removing the file from Minio");
        }
    }

    public async Task<Result<string, Error>> GetPresignedUrlAsync(
        GetPresignedUrlData data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(data.BucketName);
            
            bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
            
            if (!bucketExists)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(data.BucketName);
                
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
            
            var presignedUrlArgs = new PresignedGetObjectArgs()
                .WithBucket(data.BucketName)
                .WithObject(data.ObjectName)
                .WithExpiry((int)data.ExpirationTime.TotalSeconds);
            
            var result = await _minioClient.PresignedGetObjectAsync(presignedUrlArgs);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the presigned URL from Minio");
            return Error.Failure("file.presigned-url", "An error occurred while getting the presigned URL from Minio");
        }
    }
}