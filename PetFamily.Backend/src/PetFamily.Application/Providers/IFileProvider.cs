using CSharpFunctionalExtensions;
using PetFamily.Application.FileProvider;
using PetFamily.Domain.Shared;
using PetFamily.Domain.VolunteersManagement.ValueObjects;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    public Task<Result<List<PhotoPath>, Error>> UploadPhotosAsync(
        IEnumerable<AddPhotoData> filesData,
        CancellationToken cancellationToken = default);
    
    public Task<Result<List<string>, Error>> RemovePhotosAsync(
        IEnumerable<string> photoPaths,
        CancellationToken cancellationToken = default);
}