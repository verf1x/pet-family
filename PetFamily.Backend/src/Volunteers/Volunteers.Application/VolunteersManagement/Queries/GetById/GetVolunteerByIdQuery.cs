using PetFamily.Framework.Abstractions;

namespace Volunteers.Application.VolunteersManagement.Queries.GetById;

public record GetVolunteerByIdQuery(Guid VolunteerId) : IQuery;