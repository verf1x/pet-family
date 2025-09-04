using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.EntityIds;

public class VolunteerId : ComparableValueObject
{
    private VolunteerId(Guid value) => Value = value;
    public Guid Value { get; }

    public static VolunteerId CreateNew() => new(Guid.NewGuid());

    public static VolunteerId CreateEmpty() => new(Guid.Empty);

    public static VolunteerId Create(Guid id) => new(id);

    public static implicit operator Guid(VolunteerId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return id.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}