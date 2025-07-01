using FluentValidation;

namespace PetFamily.Application.Volunteers.Delete;

public class DeleteVolunteerRequestValidator : AbstractValidator<DeleteVolunteerRequest>
{
    public DeleteVolunteerRequestValidator()
    {
        RuleFor(r => r.VolunteerId).NotEmpty();
    }
}