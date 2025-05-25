using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public record SocialNetwork
{
    public string Name { get; }
    public string Url { get; }

    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public static Result<SocialNetwork, string> Create(string name, string url)
    {
        if(string.IsNullOrWhiteSpace(name))
            return "Social network name cannot be empty";
        
        if(string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return "Invalid URL";
        
        return new SocialNetwork(name, url);
    }
}