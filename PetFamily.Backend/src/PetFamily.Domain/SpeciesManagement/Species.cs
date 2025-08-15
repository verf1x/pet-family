using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Domain.SpeciesManagement;

public class Species : Entity<SpeciesId>
{
    private readonly List<Breed> _breeds = [];
    
    public Name Name { get; private set; } = null!;

    public IReadOnlyList<Breed> Breeds => _breeds;

    // ef core ctor
    private Species(SpeciesId id) : base(id) { }
    
    public Species(SpeciesId id, Name name) : base(id)
    {
        Name = name;
    }
    
    public void AddBreeds(List<Breed> breeds)
    {
        foreach (var breed in breeds)
            if(!_breeds.Contains(breed))
                _breeds.Add(breed);
    }
    
    public void RemoveBreeds(List<BreedId> breeds)
    {
        foreach (var breedId in breeds)
        {
            var breed = _breeds.FirstOrDefault(b => b.Id == breedId);
            if (breed != null)
                _breeds.Remove(breed);
        }
    }
}