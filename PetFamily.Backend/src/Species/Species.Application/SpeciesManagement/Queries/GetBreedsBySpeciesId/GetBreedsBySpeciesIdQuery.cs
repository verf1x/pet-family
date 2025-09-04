using PetFamily.Core.Abstractions;

namespace Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid SpeciesId) : IQuery;