using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Pet;

public record BreedSpecies
{
    public SpeciesId SpeciesId { get; private set; }
    public BreedId BreedId { get; private set; }

    private BreedSpecies(SpeciesId speciesId, BreedId breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }
    
    public static Result<BreedSpecies> Create(SpeciesId speciesId, BreedId breedId)
    {
        if (speciesId.Value == Guid.Empty)
            return Result.Failure<BreedSpecies>("Species ID cannot be empty.");

        if (breedId.Value == Guid.Empty)
            return Result.Failure<BreedSpecies>("Breed ID cannot be empty.");

        BreedSpecies breedSpecies = new(speciesId, breedId);
        
        return Result.Success(breedSpecies);
    }
}