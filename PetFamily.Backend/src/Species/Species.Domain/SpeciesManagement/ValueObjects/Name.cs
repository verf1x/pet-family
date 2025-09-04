using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace Species.Domain.SpeciesManagement.ValueObjects;

public class Name : ComparableValueObject
{
    private Name(string value) => Value = value;
    public string Value { get; }

    public static Result<Name, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Errors.General.ValueIsRequired(nameof(value));
        }

        return new Name(value);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents() =>
        throw new NotImplementedException();
}