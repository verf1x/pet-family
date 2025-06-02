using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

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
    
    public static Result<HelpDetail, Error> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired(nameof(name));
        
        if (string.IsNullOrWhiteSpace(description))
            return Errors.General.ValueIsRequired(nameof(description));
        
        return new HelpDetail(name, description);
    }
}