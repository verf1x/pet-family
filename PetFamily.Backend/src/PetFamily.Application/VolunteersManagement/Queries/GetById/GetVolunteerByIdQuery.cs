using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.Queries.GetById;

public record GetVolunteerByIdQuery(Guid VolunteerId) : IQuery;