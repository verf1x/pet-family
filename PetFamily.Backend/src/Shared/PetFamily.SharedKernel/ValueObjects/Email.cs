using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects;

public partial class Email : ComparableValueObject
{
    private Email(string value) => Value = value;
    public string Value { get; }

    public static Result<Email, Error> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex().IsMatch(email))
        {
            return Errors.General.ValueIsInvalid(nameof(email));
        }

        return new Email(email);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}