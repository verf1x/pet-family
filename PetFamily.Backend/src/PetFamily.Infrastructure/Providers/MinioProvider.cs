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
    private const int MaxParallelUploads = 5;
    
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> UploadFilesAsync(
        FilesData filesData, 
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MaxParallelUploads);

        try
        {
            await CreateBucketIfNotExists(filesData.BucketName, cancellationToken);

            var uploadTasks = new List<Task>();

            foreach (var file in filesData.Files)
            {
                await semaphoreSlim.WaitAsync(cancellationToken);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(filesData.BucketName)
                    .WithStreamData(file.Stream)
                    .WithObjectSize(file.Stream.Length)
                    .WithObject(file.ObjectName);

                var task = _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

                semaphoreSlim.Release();

                uploadTasks.Add(task);
            }

            await Task.WhenAll(uploadTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the files to Minio");
            return Error.Failure("files.upload", "An error occurred while uploading the files to Minio");
        }
        finally
        {
            semaphoreSlim.Release();
            semaphoreSlim.Dispose();
        }
        
        return UnitResult.Success<Error>();
    }

    public async Task<Result<string, Error>> RemoveFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateBucketIfNotExists(bucketName, cancellationToken);
            
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
            await CreateBucketIfNotExists(data.BucketName, cancellationToken);
            
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
    
    private async Task CreateBucketIfNotExists(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        
        var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (!bucketExists)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);
            
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}