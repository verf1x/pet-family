using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;
using PetFamily.Framework.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;

public class UpdateMainPetInfoValidator : AbstractValidator<UpdateMainPetInfoCommand>
{
    public UpdateMainPetInfoValidator()
    {
        RuleFor(u => u.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateMainPetInfoCommand.VolunteerId)));

        RuleFor(u => u.PetId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateMainPetInfoCommand.PetId)));

        RuleFor(u => u.Nickname)
            .MustBeValueObject(Nickname.Create);

        RuleFor(u => u.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(u => u.SpeciesBreed)
            .Must(sb => sb.SpeciesId != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateMainPetInfoCommand.SpeciesBreed.SpeciesId)))
            .Must(sb => sb.BreedId != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(UpdateMainPetInfoCommand.SpeciesBreed.BreedId)));

        RuleFor(u => u.Color)
            .MustBeValueObject(Color.Create);

        RuleFor(u => u.HealthInfo)
            .Must(cb => !string.IsNullOrWhiteSpace(cb.HealthStatus));

        RuleFor(u => u.Address)
            .MustBeValueObject(c =>
                Address.Create(
                    c.AddressLines.ToList(),
                    c.Locality,
                    c.Region,
                    c.PostalCode,
                    c.CountryCode));

        RuleFor(u => u.Measurements)
            .MustBeValueObject(c => Measurements.Create(c.Height, c.Weight));

        RuleFor(u => u.OwnerPhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        RuleFor(u => u.DateOfBirth)
            .NotEmpty();

        RuleForEach(c => c.HelpRequisites)
            .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description));
    }
}