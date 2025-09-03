using PetFamily.Framework.Abstractions;

namespace Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid SpeciesId) : IQuery;