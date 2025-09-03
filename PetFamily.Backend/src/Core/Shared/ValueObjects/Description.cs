using CSharpFunctionalExtensions;

namespace PetFamily.Framework.ValueObjects;

public class Description : ComparableValueObject
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(value));

        return new Description(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}