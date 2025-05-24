namespace PetFamily.Domain.ValueObjects.Volunteer;

public record VolunteerId
{
    public Guid Value { get; }
    
    private VolunteerId(Guid value) => Value = value;

    public static VolunteerId CreateNew() => new(Guid.NewGuid());

    public static VolunteerId CreateEmpty() => new(Guid.Empty);
}