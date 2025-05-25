using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Pet;

public record SpeciesBreed
{
    public SpeciesId SpeciesId { get; private set; }
    public BreedId BreedId { get; private set; }

    private SpeciesBreed(SpeciesId speciesId, BreedId breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }
    
    public static Result<SpeciesBreed, string> Create(SpeciesId speciesId, BreedId breedId)
    {
        if (speciesId.Value == Guid.Empty)
            return "Species ID cannot be empty.";

        if (breedId.Value == Guid.Empty)
            return "Breed ID cannot be empty.";
        
        return new SpeciesBreed(speciesId, breedId);
    }
}