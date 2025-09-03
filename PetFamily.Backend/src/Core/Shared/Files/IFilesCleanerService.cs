namespace PetFamily.Framework.Files;

public interface IFilesCleanerService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}