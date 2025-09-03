using FluentValidation;
using PetFamily.Framework;
using PetFamily.Framework.Validation;
using PetFamily.Framework.ValueObjects;
using PetFamily.Volunteers.Domain.VolunteersManagement.ValueObjects;

namespace Volunteers.Application.VolunteersManagement.UseCases.AddPet;

public class AddPetCommandValidator : AbstractValidator<AddPetCommand>
{
    public AddPetCommandValidator()
    {
        RuleFor(ap => ap.VolunteerId)
            .NotEqual(Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetCommand.VolunteerId)));

        RuleFor(ap => ap.Nickname)
            .MustBeValueObject(Nickname.Create);

        RuleFor(ap => ap.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(ap => ap.Color)
            .MustBeValueObject(Color.Create);

        RuleFor(ap => ap.SpeciesBreedDto)
            .Must(sb => sb.SpeciesId != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetCommand.SpeciesBreedDto.SpeciesId)))
            .Must(sb => sb.BreedId != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired(nameof(AddPetCommand.SpeciesBreedDto.BreedId)));

        RuleFor(ap => ap.HealthInfoDto)
            .Must(cb => !string.IsNullOrWhiteSpace(cb.HealthStatus));

        RuleFor(ap => ap.AddressDto)
            .MustBeValueObject(c =>
                Address.Create(
                    c.AddressLines.ToList(),
                    c.Locality,
                    c.Region,
                    c.PostalCode,
                    c.CountryCode));

        RuleFor(c => c.MeasurementsDto)
            .MustBeValueObject(c => Measurements.Create(c.Height, c.Weight));

        RuleFor(c => c.OwnerPhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        RuleFor(u => u.HelpStatus)
            .InclusiveBetween(0, 2)
            .WithError(Errors.General.ValueIsInvalid(nameof(AddPetCommand.HelpStatus)));

        RuleForEach(c => c.HelpRequisites)
            .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description));
    }
}