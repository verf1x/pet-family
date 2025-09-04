using PetFamily.Core.Abstractions;

namespace Species.Application.SpeciesManagement.UseCases.DeleteBreed;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId) : ICommand;