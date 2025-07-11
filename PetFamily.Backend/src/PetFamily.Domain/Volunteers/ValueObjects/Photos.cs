namespace PetFamily.Domain.Volunteers.ValueObjects;

public record Photos
{
    public readonly IReadOnlyList<Photo> Values;

    // ef core ctor
    private Photos() { }

    public Photos(IEnumerable<Photo> values)
    {
        Values = values.ToList();
    }
}