using CSharpFunctionalExtensions;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Photo : ComparableValueObject
{
    public Photo(string path) => Path = path;
    public string Path { get; }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Path;
    }
}