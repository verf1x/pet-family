using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.EntityIds;

public class PetId : ComparableValueObject
{
    private PetId(Guid value) => Value = value;
    public Guid Value { get; }

    public static PetId CreateNew() => new(Guid.NewGuid());

    public static PetId CreateEmpty() => new(Guid.Empty);

    public static PetId Create(Guid id) => new(id);

    public static implicit operator Guid(PetId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return id.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}