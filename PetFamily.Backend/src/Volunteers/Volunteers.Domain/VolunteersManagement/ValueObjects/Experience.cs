using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Experience : ComparableValueObject
{
    private Experience(int value) => Value = value;
    public int Value { get; }

    public static Result<Experience, Error> Create(int totalYears)
    {
        if (totalYears is > 100 or < 0)
        {
            return Errors.General.ValueIsInvalid(nameof(totalYears));
        }

        return new Experience(totalYears);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}