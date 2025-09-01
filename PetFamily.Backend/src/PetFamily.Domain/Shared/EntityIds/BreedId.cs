using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.EntityIds;

public class BreedId : ComparableValueObject
{
    public Guid Value { get; }

    private BreedId(Guid value) => Value = value;

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