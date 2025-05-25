using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects.Pet;

public record Address
{
    private const int MinAddressLines = 1;
    private const int MaxAddressLines = 4;
    
    public IReadOnlyList<string> AddressLines { get; }
    public string Locality { get; }
    public string? Region { get; }
    public string? PostalCode { get; }
    public string CountryCode { get; }

    private Address(IReadOnlyList<string> addressLines, string locality, string region, string postalCode, string countryCode)
    {
        AddressLines = addressLines;
        Locality = locality;
        Region = region;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }
    
    public static Result<Address, string> Create(
        List<string> addressLines,
        string locality,
        string region,
        string postalCode,
        string countryCode)
    {
        if(addressLines.Count is < MinAddressLines or > MaxAddressLines)
            return $"The number of address lines must be between {MinAddressLines} and {MaxAddressLines} to comply" +
                   $" with the 1-4 address line standard (street, house, building, etc.)";
        
        if (string.IsNullOrWhiteSpace(locality))
            return "Locality cannot be empty.";

        if (string.IsNullOrWhiteSpace(countryCode) || !Regex.IsMatch(countryCode, @"^[A-Z]{2}$"))
            return "Country code must be a 2-letter ISO 3166-1 alpha-2 code.";
        
        return new Address(addressLines, locality, region, postalCode, countryCode);
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