using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects;

public record HelpDetails
{
    public string Name { get; }
    public string Description { get; } 
    
    private HelpDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public static Result<HelpDetails> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<HelpDetails>("Name cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<HelpDetails>("Description cannot be empty.");
        
        HelpDetails helpDetails = new(name, description);
        
        return Result.Success(helpDetails);
    }
}