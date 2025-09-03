using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.Queries.GetPetById;

public record GetPetByIdQuery(Guid PetId) : IQuery;