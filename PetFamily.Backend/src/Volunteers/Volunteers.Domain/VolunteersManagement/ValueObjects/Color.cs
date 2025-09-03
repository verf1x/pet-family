using CSharpFunctionalExtensions;
using PetFamily.Framework;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Color : ComparableValueObject
{
    public string Value { get; }

    private Color(string value) => Value = value;

    public static Result<Color, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(value));

        return new Color(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}