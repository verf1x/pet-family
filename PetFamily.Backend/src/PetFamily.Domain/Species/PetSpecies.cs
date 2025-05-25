using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Pet;

namespace PetFamily.Domain.Species;

public class PetSpecies : Shared.Entity<SpeciesId>
{
    public string Name { get; private set; } = null!;
    public Breeds Breeds { get; private set; } = null!;

    // ef core ctor
    private PetSpecies(SpeciesId id) : base(id) { }
    
    private PetSpecies(SpeciesId id, string name, Breeds breeds) : base(id)
    {
        Name = name;
    }

    public static Result<PetSpecies> Create(SpeciesId id, string name, Breeds breeds)
    {
        if(string.IsNullOrWhiteSpace(name))
            return "Name cannot be null or whitespace.";
        
        return new PetSpecies(id, name, breeds);
    }
}