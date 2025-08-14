using FluentValidation;

namespace PetFamily.Application.SpeciesManagement.UseCases.AddBreeds;

public class AddBreedsCommandValidator : AbstractValidator<AddBreedsCommand>
{
    public AddBreedsCommandValidator()
    {
    }
}