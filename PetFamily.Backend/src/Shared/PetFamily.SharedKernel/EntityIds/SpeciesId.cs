using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.EntityIds;

public class SpeciesId : ComparableValueObject
{
    private SpeciesId(Guid value) => Value = value;
    public Guid Value { get; }

    public static SpeciesId CreateNew() => new(Guid.NewGuid());

    public static SpeciesId CreateEmpty() => new(Guid.Empty);

    public static SpeciesId Create(Guid id) => new(id);

    public static implicit operator Guid(SpeciesId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return id.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}