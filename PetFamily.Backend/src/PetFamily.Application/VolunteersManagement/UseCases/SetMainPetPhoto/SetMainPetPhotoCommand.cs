using PetFamily.Application.Abstractions;

namespace PetFamily.Application.VolunteersManagement.UseCases.SetMainPetPhoto;

public record SetMainPetPhotoCommand(Guid VolunteerId, Guid PetId, string PhotoPath) : ICommand;