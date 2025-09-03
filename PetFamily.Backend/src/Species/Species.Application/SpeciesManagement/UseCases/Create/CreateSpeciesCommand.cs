using PetFamily.Framework.Abstractions;

namespace Species.Application.SpeciesManagement.UseCases.Create;

public record CreateSpeciesCommand(string Name) : ICommand;