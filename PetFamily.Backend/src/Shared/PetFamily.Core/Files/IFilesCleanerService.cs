namespace PetFamily.Core.Files;

public interface IFilesCleanerService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}