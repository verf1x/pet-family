namespace PetFamily.Application.Dtos;

public class PetDto
{
    public Guid Id { get; set; }
    
    public Guid VolunteerId { get; set; }
    
    public string Nickname { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int Position { get; set; }
    
    public string Color { get; set; } = null!;

    public PetPhotoDto[] Photos { get; set; } = null!;
}

public class PetPhotoDto
{
    public string PhotoPath { get; init; } = string.Empty;
}