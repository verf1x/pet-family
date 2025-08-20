using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public class File : ComparableValueObject
{
    public FilePath Path { get; }

    public File(FilePath path) => Path = path;

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Path;
    }
}