using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record FullName
{
    public string FirstName { get; } 
    public string LastName { get; } 
    public string? MiddleName { get; }

    private FullName(string firstName, string lastName, string middleName)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public static Result<FullName> Create(string firstName, string lastName, string middleName = null!)
    {
        if(string.IsNullOrWhiteSpace(firstName))
            return "First name cannot be empty";
        
        if(string.IsNullOrWhiteSpace(lastName))
            return "Last name cannot be empty";
        
        return new FullName(firstName, lastName, middleName);
    }
}