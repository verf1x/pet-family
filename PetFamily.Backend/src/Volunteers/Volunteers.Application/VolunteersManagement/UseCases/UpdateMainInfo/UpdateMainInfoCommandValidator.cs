using FluentValidation;
using PetFamily.Core.Validation;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdateMainInfo;

public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoCommand>
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