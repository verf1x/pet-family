using CSharpFunctionalExtensions;
using PetFamily.Framework;
using PetFamily.Framework.EntityIds;

namespace Species.Domain.SpeciesManagement.ValueObjects;

public class Breed : ComparableValueObject
{
    public BreedId Id { get; private set; }

    public string Name { get; private set; }

    private Breed(string name)
    {
        Id = BreedId.CreateNew();
        Name = name;
    }

    public static Result<Breed, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired(nameof(name));

        return new Breed(name);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Id;
        yield return Name;
    }
}