using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Dtos;

public class PetDto
{
    public Guid Id { get; init; }
    
    public Guid VolunteerId { get; init; }
    
    public string Nickname { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public int Position { get; private set; }
    
    public string Color { get; init; } = null!;

    public PetPhotoDto[] Photos { get; private set; } = null!;
}

public class PetPhotoDto
{
    public string PhotoPath { get; set; } = string.Empty;
}