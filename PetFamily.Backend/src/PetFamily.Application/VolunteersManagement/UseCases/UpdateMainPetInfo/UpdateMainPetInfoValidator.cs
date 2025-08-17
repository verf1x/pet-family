using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetInfo;

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

        RuleFor(c => c.HealthInfo)
            .Must(cb => !string.IsNullOrWhiteSpace(cb.HealthStatus));

        RuleFor(c => c.Address)
            .MustBeValueObject(c =>
                Address.Create(
                    c.AddressLines.ToList(),
                    c.Locality,
                    c.Region,
                    c.PostalCode,
                    c.CountryCode));

        RuleFor(c => c.Measurements)
            .MustBeValueObject(c => Measurements.Create(c.Height, c.Weight));

        RuleFor(c => c.OwnerPhoneNumber)
            .MustBeValueObject(PhoneNumber.Create);

        RuleFor(c => c.DateOfBirth)
            .NotEmpty();

        RuleForEach(c => c.HelpRequisites)
            .MustBeValueObject(hr => HelpRequisite.Create(hr.Name, hr.Description));
    }
}