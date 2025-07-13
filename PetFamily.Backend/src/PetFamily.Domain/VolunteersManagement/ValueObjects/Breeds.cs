using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

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