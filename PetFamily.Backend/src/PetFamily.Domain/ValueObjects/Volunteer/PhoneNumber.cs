using System.Text.RegularExpressions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record PhoneNumber
{
    public string Number { get; }
    
    private PhoneNumber(string number)
    {
        Number = number;
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            !Regex.IsMatch(phoneNumber, @"^\+?\d{1,4}?[\s-]?(\(?\d{1,5}\)?[\s-]?)?[\d\s-]{5,15}$"))
        {
            return "Invalid phone number";
        }

        return new PhoneNumber(phoneNumber);
    }
}