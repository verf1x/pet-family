using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;

namespace PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

public class SocialNetwork : ComparableValueObject
{
    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public string Name { get; }

    public string Url { get; }

    public static Result<SocialNetwork, Error> Create(string name, string url)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Errors.General.ValueIsRequired(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return Errors.General.ValueIsInvalid(nameof(url));
        }

        return new SocialNetwork(name, url);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Name;
        yield return Url;
    }
}