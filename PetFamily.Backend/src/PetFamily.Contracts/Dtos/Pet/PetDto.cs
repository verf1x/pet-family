namespace PetFamily.Contracts.Dtos.Pet;

public class PetDto
{
    public Guid Id { get; init; }
    
    public Guid VolunteerId { get; init; }
    
    public string Nickname { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public int Position { get; init; }
    
    public string Color { get; init; } = null!;

    public PetFileDto[] Photos { get; set; } = null!;
}

public class PetFileDto
{
    public string FilePath { get; init; } = string.Empty;
}