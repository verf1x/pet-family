using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects;

public record HelpDetail
{
    public string Name { get; }
    public string Description { get; } 
    
    private HelpDetail(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public static Result<HelpDetail, string> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name cannot be empty.";
        
        if (string.IsNullOrWhiteSpace(description))
            return "Description cannot be empty.";
        
        return new HelpDetail(name, description);
    }
}