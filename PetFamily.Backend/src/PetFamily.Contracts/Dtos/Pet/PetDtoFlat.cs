namespace PetFamily.Contracts.Dtos.Pet;

public class PetDtoFlat
{
    public Guid Id { get; init; }

    public Guid VolunteerId { get; init; }

    public string Nickname { get; init; } = null!;

    public DateTime BirthDate { get; init; }

    public Guid SpeciesId { get; init; }

    public Guid BreedId { get; init; }

    public string AddressLines { get; init; } = null!;

    public string Locality { get; init; } = null!;

    public string? Region { get; init; }

    public string? PostalCode { get; init; }

    public string CountryCode { get; init; } = null!;

    public float Weight { get; init; }

    public float Height { get; init; }

    public int HelpStatus { get; init; }

    public string Description { get; init; } = null!;

    public int Position { get; init; }

    public string Color { get; init; } = null!;
}