namespace PetFamily.Contracts.Dtos.Species;

public class SpeciesDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; } = null!;
    
    public BreedDto[] Breeds { get; set; } = null!;
}