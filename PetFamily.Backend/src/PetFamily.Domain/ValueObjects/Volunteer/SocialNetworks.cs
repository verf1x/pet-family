namespace PetFamily.Domain.ValueObjects.Volunteer;

public record SocialNetworks
{
    private readonly List<SocialNetwork> _values;
    
    public IReadOnlyList<SocialNetwork> Values => _values;
    
    // ef core ctor
    private SocialNetworks() { }
    
    public SocialNetworks(List<SocialNetwork> values)
    {
        _values = values;
    }
}