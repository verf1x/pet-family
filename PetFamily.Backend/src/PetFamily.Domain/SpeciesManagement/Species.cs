using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Domain.SpeciesManagement;

public class Species : Entity<SpeciesId>
{
    public string Name { get; private set; } = null!;
    public IReadOnlyList<Breed> Breeds { get; private set; } = null!;

    // ef core ctor
    private Species(SpeciesId id) : base(id) { }
    
    private Species(SpeciesId id, string name, IReadOnlyList<Breed> breeds) : base(id)
    {
        Name = name;
    }

    public static Result<Species, Error> Create(SpeciesId id, string name, IReadOnlyList<Breed> breeds)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired(nameof(name));
        
        return new Species(id, name, breeds);
    }
}