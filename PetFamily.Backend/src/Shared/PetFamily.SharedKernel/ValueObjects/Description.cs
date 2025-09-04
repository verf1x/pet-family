using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects;

public class Description : ComparableValueObject
{
    private Description(string value) => Value = value;
    public string Value { get; }

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Errors.General.ValueIsRequired(nameof(value));
        }

        return new Description(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}