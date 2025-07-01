using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;
using PetFamily.Domain.ValueObjects.Volunteer;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoRequestValidator : AbstractValidator<UpdateMainInfoRequest>
{
    public UpdateMainInfoRequestValidator()
    {
        RuleFor(r => r.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}

public class UpdateMainInfoDtoValidator : AbstractValidator<UpdateMainInfoDto>
{
    public UpdateMainInfoDtoValidator()
    {
        RuleFor(d => d.FullName)
            .MustBeValueObject(fn => FullName.Create(fn.FirstName, fn.LastName, fn.MiddleName));
        
        RuleFor(d => d.Email)
            .MustBeValueObject(Email.Create);
        
        RuleFor(d => d.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(d => d.ExperienceYears)
            .MustBeValueObject(Experience.Create);

        RuleFor(d => d.PhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);
    }
}