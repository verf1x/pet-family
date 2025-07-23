using PetFamily.Application.Dtos;
using PetFamily.Application.Volunteers.AddPet;

namespace PetFamily.Api.Processors;

public class FormFileProcessor : IAsyncDisposable
{
    private readonly List<UploadFileDto> _fileDtos = [];

    public List<UploadFileDto> Process(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            var stream = file.OpenReadStream();
            var fileDto = new UploadFileDto(stream, file.FileName);
            _fileDtos.Add(fileDto);
        }
        
        return _fileDtos;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var fileDto in _fileDtos)
            await fileDto.Content.DisposeAsync();
    }
}