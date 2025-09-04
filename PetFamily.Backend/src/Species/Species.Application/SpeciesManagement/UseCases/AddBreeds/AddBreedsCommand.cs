using PetFamily.Core.Abstractions;

namespace Species.Application.SpeciesManagement.UseCases.AddBreeds;

public record AddBreedsCommand(Guid SpeciesId, IEnumerable<string> BreedsNames) : ICommand;