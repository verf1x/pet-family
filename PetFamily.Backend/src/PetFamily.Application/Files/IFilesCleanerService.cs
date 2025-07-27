namespace PetFamily.Application.Files;

public interface IFilesCleanerService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}