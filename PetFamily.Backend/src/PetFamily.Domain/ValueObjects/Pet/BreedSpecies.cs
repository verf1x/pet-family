using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Pet;

public record BreedSpecies
{
    public Guid SpeciesId { get; private set; }
    public Guid BreedId { get; private set; }

    private BreedSpecies(Guid speciesId, Guid breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }
    
    public static Result<BreedSpecies> Create(Guid speciesId, Guid breedId)
    {
        if (speciesId == Guid.Empty)
            return Result.Failure<BreedSpecies>("Species ID cannot be empty.");

        if (breedId == Guid.Empty)
            return Result.Failure<BreedSpecies>("Breed ID cannot be empty.");

        BreedSpecies breedSpecies = new(speciesId, breedId);
        
        return Result.Success(breedSpecies);
    }
}