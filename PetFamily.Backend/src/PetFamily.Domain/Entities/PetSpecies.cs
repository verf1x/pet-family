using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Species;
using PetFamily.Domain.ValueObjects.Pet;

namespace PetFamily.Domain.Entities;

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

    public static Result<PetSpecies, Error> Create(SpeciesId id, string name, Breeds breeds)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired(nameof(name));
        
        return new PetSpecies(id, name, breeds);
    }
}