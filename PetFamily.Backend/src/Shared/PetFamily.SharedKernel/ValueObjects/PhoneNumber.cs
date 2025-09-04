using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects;

public class PhoneNumber : ComparableValueObject
{
    private PhoneNumber(string value) => Value = value;
    public string Value { get; }

    public static Result<PhoneNumber, Error> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            !Regex.IsMatch(phoneNumber, @"^\+?\d{1,4}?[\s-]?(\(?\d{1,5}\)?[\s-]?)?[\d\s-]{5,15}$"))
        {
            return Errors.General.ValueIsInvalid(nameof(phoneNumber));
        }

        return new PhoneNumber(phoneNumber);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}