using PetFamily.Application.Abstractions;

namespace PetFamily.Application.SpeciesManagement.UseCases.AddBreeds;

public record AddBreedsCommand(Guid SpeciesId, IEnumerable<string> BreedsNames) : ICommand;