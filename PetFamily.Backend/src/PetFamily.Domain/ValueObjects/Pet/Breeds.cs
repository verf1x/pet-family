using PetFamily.Domain.Species;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Breeds
{
    public readonly IReadOnlyList<Breed> Values;

    // ef core ctor
    private Breeds() { }
    
    public Breeds(IEnumerable<Breed> values)
    {
        Values = values.ToList();
    }
}