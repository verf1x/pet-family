using PetFamily.Application.Abstractions;

namespace PetFamily.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid SpeciesId) : IQuery;