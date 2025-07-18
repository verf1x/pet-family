namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public record Photo
{
    public PhotoPath PhotoPath { get; }
    
    public Photo(PhotoPath photoPath) => PhotoPath = photoPath;
}