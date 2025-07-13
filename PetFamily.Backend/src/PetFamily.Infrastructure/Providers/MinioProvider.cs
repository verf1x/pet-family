using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

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

    public async Task<Result<IReadOnlyList<FilePath>, Error>> UploadFilesAsync(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MaxParallelUploads);
        var files = filesData.ToList();

        try
        { 
            await CreateBucketsIfNotExistsAsync(files, cancellationToken);

            var tasks = files.Select(async file =>
                await PutObjectAsync(file, semaphoreSlim, cancellationToken));
            
            var pathsResult = await Task.WhenAll(tasks);
            
            if(pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;
            
            var result = pathsResult.Select(p => p.Value).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading the files to Minio");
            return Error.Failure("files.upload", "An error occurred while uploading the files to Minio");
        }
    }

    private async Task<Result<FilePath, Error>> PutObjectAsync(
        FileData fileData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken = default)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(fileData.BucketName)
            .WithStreamData(fileData.Stream)
            .WithObjectSize(fileData.Stream.Length)
            .WithObject(fileData.FilePath.Path);

        try
        {
            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            return fileData.FilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to MinIO with path {path} in bucket {bucket}",
                fileData.FilePath.Path,
                fileData.BucketName);
            
            return Error.Failure("file.upload", "An error occurred while uploading the file to Minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
    
    private async Task CreateBucketsIfNotExistsAsync(
        IEnumerable<FileData> filesData,
        CancellationToken cancellationToken = default)
    {
        HashSet<string> bucketNames = [..filesData.Select(file => file.BucketName)];

        foreach (var bucketName in bucketNames)
        {
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            var bucketExist = await _minioClient
                .BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
        }
    }
}