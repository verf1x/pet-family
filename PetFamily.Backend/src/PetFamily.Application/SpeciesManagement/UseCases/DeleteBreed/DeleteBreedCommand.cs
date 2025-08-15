using PetFamily.Application.Abstractions;

namespace PetFamily.Application.SpeciesManagement.UseCases.DeleteBreed;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId) : ICommand;