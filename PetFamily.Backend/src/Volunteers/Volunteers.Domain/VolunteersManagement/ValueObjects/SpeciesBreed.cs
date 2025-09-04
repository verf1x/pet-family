using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class SpeciesBreed : ComparableValueObject
{
    private SpeciesBreed(SpeciesId speciesId, BreedId breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public SpeciesId SpeciesId { get; }

    public BreedId BreedId { get; }

    public static Result<SpeciesBreed, Error> Create(SpeciesId speciesId, BreedId breedId) =>
        // TODO:
        // if (speciesId.Value == Guid.Empty)
        //     return Errors.General.ValueIsInvalid(nameof(speciesId));
        //
        // if (breedId.Value == Guid.Empty)
        //     return Errors.General.ValueIsInvalid(nameof(breedId));
        new SpeciesBreed(speciesId, breedId);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return SpeciesId;
        yield return BreedId;
    }
}