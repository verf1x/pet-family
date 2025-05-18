using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Volunteer;

public class SocialNetwork : ValueObject
{
    public string Name { get; }
    public string Url { get; }

    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public static Result<SocialNetwork> Create(string name, string url)
    {
        if(string.IsNullOrWhiteSpace(name))
            return Result.Failure<SocialNetwork>("Social network name cannot be empty");
        
        if(string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return Result.Failure<SocialNetwork>("Invalid URL");
        
        SocialNetwork socialNetwork = new(name, url);
        
        return Result.Success(socialNetwork);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Url;
    }
}