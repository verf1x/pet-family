using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record SocialNetworks
{
    private readonly List<SocialNetwork> _values = [];
    
    public IReadOnlyList<SocialNetwork> Values => _values;
    
    public Result<Error> AddSocialNetwork(SocialNetwork socialNetwork)
    {
        if (_values.Contains(socialNetwork))
            return Errors.General.Conflict();
        
        _values.Add(socialNetwork);
        
        return Result.Success<Error>(null!);
    }
}