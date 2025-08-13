using PetFamily.Application.Abstractions;

namespace PetFamily.Application.SpeciesManagement.UseCases.Create;

public record CreateSpeciesCommand(string Name) : ICommand;