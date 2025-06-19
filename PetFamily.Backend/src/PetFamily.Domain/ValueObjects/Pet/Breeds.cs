using PetFamily.Domain.Species;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Breeds
{
    public readonly IReadOnlyList<Breed> Values;

    public Breeds(IEnumerable<Breed> values)
    {
        Values = values.ToList();
    }
}