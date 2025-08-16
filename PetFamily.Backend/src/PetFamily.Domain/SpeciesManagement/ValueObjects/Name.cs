using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.SpeciesManagement.ValueObjects;

public class Name : ComparableValueObject
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(value));

        return new Name(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        throw new NotImplementedException();
    }
}