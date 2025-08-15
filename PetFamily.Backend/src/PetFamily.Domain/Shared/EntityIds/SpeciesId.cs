using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared.EntityIds;

public class SpeciesId : ComparableValueObject
{
    public Guid Value { get; }
    
    private SpeciesId(Guid value) => Value = value;

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