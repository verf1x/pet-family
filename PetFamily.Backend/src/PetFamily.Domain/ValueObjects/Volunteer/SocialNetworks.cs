using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record SocialNetworks
{
    private readonly List<SocialNetwork> _all = [];
    
    public IReadOnlyList<SocialNetwork> All => _all;
    
    public Result AddSocialNetwork(SocialNetwork socialNetwork)
    {
        if (_all.Contains(socialNetwork))
            return Result.Failure("Social network already exists in the volunteer's list.");
        
        _all.Add(socialNetwork);
        
        return Result.Success();
    }
}