using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.EntityIds;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public class SpeciesBreed : ComparableValueObject
{
    public SpeciesId SpeciesId { get; }

    public BreedId BreedId { get; }

    private SpeciesBreed(SpeciesId speciesId, BreedId breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public static Result<SpeciesBreed, Error> Create(SpeciesId speciesId, BreedId breedId)
    {
        // TODO:
        // if (speciesId.Value == Guid.Empty)
        //     return Errors.General.ValueIsInvalid(nameof(speciesId));
        //
        // if (breedId.Value == Guid.Empty)
        //     return Errors.General.ValueIsInvalid(nameof(breedId));

        return new SpeciesBreed(speciesId, breedId);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return SpeciesId;
        yield return BreedId;
    }
}