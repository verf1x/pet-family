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