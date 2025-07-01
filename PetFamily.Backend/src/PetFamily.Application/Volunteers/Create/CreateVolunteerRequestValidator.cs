using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.ValueObjects;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers.Create;

public class CreateVolunteerRequestValidator : AbstractValidator<CreateVolunteerRequest>
{
    public CreateVolunteerRequestValidator()
    {
        RuleFor(r => r.FullName)
            .MustBeValueObject(fn => FullName.Create(fn.FirstName, fn.LastName, fn.MiddleName!));
        
        RuleFor(r => r.Email)
            .MustBeValueObject(Email.Create);

        RuleFor(r => r.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(r => r.ExperienceYears)
            .MustBeValueObject(Experience.Create);
        
        RuleFor(r => r.PhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        RuleForEach(r => r.SocialNetworks)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Url));

        RuleForEach(r => r.HelpRequisites)
            .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description));
    }
}