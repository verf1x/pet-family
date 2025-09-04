using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.EntityIds;

public class BreedId : ComparableValueObject
{
    private BreedId(Guid value) => Value = value;
    public Guid Value { get; }

    public static BreedId CreateNew() => new(Guid.NewGuid());

    public static BreedId CreateEmpty() => new(Guid.Empty);

    public static BreedId Create(Guid id) => new(id);

    public static implicit operator Guid(BreedId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return id.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}