using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using PetFamily.Core.Files;
using PetFamily.SharedKernel;

namespace Infrastructure.S3;

public class MinioProvider : IFileProvider
{
    private const int MaxParallelOperations = 5;
    private const string BucketName = "photos";
    private readonly ILogger<MinioProvider> _logger;

    private readonly IMinioClient _minioClient;

    public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<List<string>, Error>> UploadPhotosAsync(
        IEnumerable<PhotoData> filesData,
        CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphoreSlim = new(MaxParallelOperations);
        List<PhotoData> files = filesData.ToList();

        try
        {
            await CreateBucketsIfNotExistsAsync(cancellationToken);

            IEnumerable<Task<Result<string, Error>>> tasks = files.Select(async file =>
                await PutObjectAsync(file, semaphoreSlim, cancellationToken));

            Result<string, Error>[] pathsResult = await Task.WhenAll(tasks);

            if (pathsResult.Any(p => p.IsFailure))
            {
                return pathsResult.First().Error;
            }

            List<string> result = pathsResult.Select(p => p.Value).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the files to Minio");
            return Error.Failure(
                "files.upload",
                "An error occurred while uploading the files to Minio");
        }
    }

    public async Task<Result<List<string>, Error>> RemoveFilesAsync(
        IEnumerable<string> filesPaths,
        CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphoreSlim = new(MaxParallelOperations);
        List<string> fileNames = filesPaths.ToList();

        try
        {
            IEnumerable<Task<Result<string, Error>>> tasks = fileNames.Select(async objectName =>
                await RemoveObjectAsync(objectName, semaphoreSlim, cancellationToken));

            Result<string, Error>[] fileNamesResult = await Task.WhenAll(tasks);

            if (fileNamesResult.Any(r => r.IsFailure))
            {
                return fileNamesResult.First().Error;
            }

            List<string> result = fileNamesResult.Select(p => p.Value).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while removing the files from Minio");
            return Error.Failure(
                "files.remove",
                "An error occurred while removing the files from Minio");
        }
    }

    public async Task<UnitResult<Error>> RemoveFileAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateBucketsIfNotExistsAsync(cancellationToken);

            StatObjectArgs? statArgs = new StatObjectArgs()
                .WithBucket(BucketName)
                .WithObject(path);

            ObjectStat? objectStat = await _minioClient.StatObjectAsync(statArgs, cancellationToken);
            if (objectStat is null)
            {
                return UnitResult.Success<Error>();
            }

            RemoveObjectArgs? args = new RemoveObjectArgs()
                .WithBucket(BucketName)
                .WithObject(path);

            await _minioClient.RemoveObjectAsync(args, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload file to MinIO with path {path} in bucket {bucket}",
                path,
                BucketName);

            return Error.Failure("file.remove", "Failed to remove file from MinIO");
        }

        return Result.Success<Error>();
    }

    private async Task<Result<string, Error>> PutObjectAsync(
        PhotoData photoData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken = default)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        PutObjectArgs? putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithStreamData(photoData.Stream)
            .WithObjectSize(photoData.Stream.Length)
            .WithObject(photoData.Path);

        try
        {
            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            return photoData.Path;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload file to MinIO with path {path} in bucket {bucket}",
                photoData.Path,
                BucketName);

            return Error.Failure(
                "file.upload",
                "An error occurred while uploading the file to Minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task<Result<string, Error>> RemoveObjectAsync(
        string objectName,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken = default)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        RemoveObjectArgs? removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(BucketName)
            .WithObject(objectName);

        try
        {
            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to remove file from MinIO with path {path} in bucket {bucket}",
                objectName,
                BucketName);

            return Error.Failure(
                "file.remove",
                "An error occurred while removing the file from Minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task CreateBucketsIfNotExistsAsync(
        CancellationToken cancellationToken = default)
    {
        BucketExistsArgs? bucketExistArgs = new BucketExistsArgs()
            .WithBucket(BucketName);

        bool bucketExist = await _minioClient
            .BucketExistsAsync(bucketExistArgs, cancellationToken);

        if (!bucketExist)
        {
            MakeBucketArgs? makeBucketArgs = new MakeBucketArgs()
                .WithBucket(BucketName);

            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}