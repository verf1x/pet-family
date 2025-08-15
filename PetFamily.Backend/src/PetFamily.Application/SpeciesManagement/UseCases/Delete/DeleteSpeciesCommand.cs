using PetFamily.Application.Abstractions;

namespace PetFamily.Application.SpeciesManagement.UseCases.Delete;

public record DeleteSpeciesCommand(Guid SpeciesId) : ICommand;