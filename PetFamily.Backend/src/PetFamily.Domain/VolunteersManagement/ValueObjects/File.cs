namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public record File
{
    public FilePath FilePath { get; }
    
    public File(FilePath filePath) => FilePath = filePath;
}