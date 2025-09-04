using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class Nickname : ComparableValueObject
{
    private Nickname(string value) => Value = value;
    public string Value { get; }

    public static Result<Nickname, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Errors.General.ValueIsRequired(nameof(value));
        }

        return new Nickname(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}