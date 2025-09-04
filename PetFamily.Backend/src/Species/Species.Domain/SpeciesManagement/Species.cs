using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.EntityIds;
using Species.Domain.SpeciesManagement.ValueObjects;

namespace Species.Domain.SpeciesManagement;

public class Species : Entity<SpeciesId>
{
    private readonly List<Breed> _breeds = [];

    public Species(SpeciesId id, Name name)
        : base(id) =>
        Name = name;

    // ef core ctor
    private Species(SpeciesId id)
        : base(id)
    {
    }

    public Name Name { get; private set; } = null!;

    public IReadOnlyList<Breed> Breeds => _breeds;

    public void AddBreeds(List<Breed> breeds)
    {
        foreach (Breed breed in breeds)
        {
            if (!_breeds.Contains(breed))
            {
                _breeds.Add(breed);
            }
        }
    }

    public void RemoveBreeds(List<BreedId> breeds)
    {
        foreach (BreedId breedId in breeds)
        {
            Breed? breed = _breeds.FirstOrDefault(b => b.Id == breedId);
            if (breed != null)
            {
                _breeds.Remove(breed);
            }
        }
    }
}