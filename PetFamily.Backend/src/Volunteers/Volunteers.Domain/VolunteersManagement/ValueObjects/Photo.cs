using CSharpFunctionalExtensions;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Photo : ComparableValueObject
{
    public string Path { get; }

    public Photo(string path) => Path = path;

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Path;
    }
}