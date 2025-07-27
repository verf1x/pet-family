using CSharpFunctionalExtensions;

namespace PetFamily.Domain.VolunteersManagement.ValueObjects;

public class Photo : ComparableValueObject
{
    public PhotoPath PhotoPath { get; }
    
    public Photo(PhotoPath photoPath) => PhotoPath = photoPath;
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return PhotoPath;
    }
}