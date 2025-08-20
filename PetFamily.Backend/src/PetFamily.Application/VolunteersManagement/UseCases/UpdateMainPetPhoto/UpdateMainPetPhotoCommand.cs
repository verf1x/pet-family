using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.UpdateMainPetPhoto;

public record UpdateMainPetPhotoCommand(Guid VolunteerId, Guid PetId, string photoPath) : ICommand;