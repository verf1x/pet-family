using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.Volunteers.ValueObjects;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoDto>
{
    public UpdateMainInfoCommandValidator()
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

public class UpdateMainInfoDtoValidator : AbstractValidator<UpdateMainInfoCommand>
{
    public UpdateMainInfoDtoValidator()
    {
        RuleFor(r => r.VolunteerId)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}