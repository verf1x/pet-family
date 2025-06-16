using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record SocialNetworks
{
    private readonly List<SocialNetwork> _values;
    
    public IReadOnlyList<SocialNetwork> Values => _values;
    
    // ef core ctor
    private SocialNetworks() {}
    
    private SocialNetworks(List<SocialNetwork> values)
    {
        _values = values;
    }
    
    public static Result<SocialNetworks, Error> Create(IEnumerable<SocialNetwork> socialNetworks)
    {
        return Result.Success<SocialNetworks, Error>(new SocialNetworks(socialNetworks.ToList()));
    }
}