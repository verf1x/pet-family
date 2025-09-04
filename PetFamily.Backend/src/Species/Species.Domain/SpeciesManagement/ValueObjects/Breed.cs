using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.EntityIds;

namespace Species.Domain.SpeciesManagement.ValueObjects;

public class Breed : ComparableValueObject
{
    private Breed(string name)
    {
        Id = BreedId.CreateNew();
        Name = name;
    }

    public BreedId Id { get; }

    public string Name { get; }

    public static Result<Breed, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.General.ValueIsRequired(nameof(name));
        }

        return new Breed(name);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Id;
        yield return Name;
    }
}