using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Address
{
    private const int MinAddressLines = 1;
    private const int MaxAddressLines = 4;
    private readonly List<string> _addressLines;
    
    public IReadOnlyList<string> AddressLines => _addressLines;
    public string Locality { get; }
    public string? Region { get; }
    public string? PostalCode { get; }
    public string CountryCode { get; }

    private Address(List<string> addressLines, string locality, string region, string postalCode, string countryCode)
    {
        _addressLines = addressLines;
        Locality = locality;
        Region = region;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }
    
    public static Result<Address> Create(
        List<string> addressLines,
        string locality,
        string region,
        string postalCode,
        string countryCode)
    {
        if(addressLines.Count is < MinAddressLines or > MaxAddressLines)
            return Result.Failure<Address>(
                $"The number of address lines must be between {MinAddressLines} and {MaxAddressLines} to comply" +
                $" with the 1-4 address line standard (street, house, building, etc.)");
        
        if (string.IsNullOrWhiteSpace(locality))
            return Result.Failure<Address>("Locality cannot be empty.");

        if (string.IsNullOrWhiteSpace(countryCode) || !Regex.IsMatch(countryCode, @"^[A-Z]{2}$"))
            return Result.Failure<Address>("Country code must be a 2-letter ISO 3166-1 alpha-2 code.");
        
        Address address = new(addressLines, locality, region, postalCode, countryCode);
        
        return Result.Success(address);
    }
    
    public override string ToString()
    {
        List<string> parts = [];
        parts.AddRange(AddressLines);
        parts.Add(Locality);
        
        if (!string.IsNullOrEmpty(Region))     
            parts.Add(Region!);
        
        if (!string.IsNullOrEmpty(PostalCode)) 
            parts.Add(PostalCode!);
        
        parts.Add(CountryCode);
        return string.Join(", ", parts);
    }
}