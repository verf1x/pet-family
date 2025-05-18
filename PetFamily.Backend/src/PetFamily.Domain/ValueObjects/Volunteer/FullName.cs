using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public class FullName : ValueObject
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
            return Result.Failure<FullName>("First name cannot be empty");
        
        if(string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<FullName>("Last name cannot be empty");

        FullName name = new(firstName, lastName, middleName);
        
        return Result.Success(name);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        
        if (MiddleName is not null) 
            yield return MiddleName;
    }
}