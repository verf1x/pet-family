using PetFamily.Core.Abstractions;

namespace Species.Application.SpeciesManagement.UseCases.Delete;

public record DeleteSpeciesCommand(Guid SpeciesId) : ICommand;