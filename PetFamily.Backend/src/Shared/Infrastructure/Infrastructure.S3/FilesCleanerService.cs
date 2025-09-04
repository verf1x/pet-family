using Microsoft.Extensions.Logging;
using PetFamily.Core.Files;
using PetFamily.Core.Messaging;

namespace Infrastructure.S3;

public class FilesCleanerService : IFilesCleanerService
{
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<IFilesCleanerService> _logger;
    private readonly IMessageQueue<IEnumerable<string>> _messageQueue;

    public FilesCleanerService(
        IFileProvider fileProvider,
        ILogger<FilesCleanerService> logger,
        IMessageQueue<IEnumerable<string>> messageQueue)
    {
        _fileProvider = fileProvider;
        _logger = logger;
        _messageQueue = messageQueue;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        IEnumerable<string> filesPaths = await _messageQueue.ReadAsync(cancellationToken);

        foreach (string fileInfo in filesPaths)
        {
            await _fileProvider.RemoveFileAsync(fileInfo, cancellationToken);
        }
    }
}