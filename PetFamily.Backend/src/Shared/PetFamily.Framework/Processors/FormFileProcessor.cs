using Microsoft.AspNetCore.Http;
using Volunteers.Contracts.Dtos;

namespace PetFamily.Framework.Processors;

public class FormFileProcessor : IAsyncDisposable
{
    private readonly List<UploadFileDto> _fileDtos = [];

    public async ValueTask DisposeAsync()
    {
        foreach (UploadFileDto fileDto in _fileDtos)
        {
            await fileDto.Content.DisposeAsync();
        }
    }

    public List<UploadFileDto> Process(IFormFileCollection files)
    {
        foreach (IFormFile file in files)
        {
            Stream stream = file.OpenReadStream();
            UploadFileDto fileDto = new(stream, file.FileName);
            _fileDtos.Add(fileDto);
        }

        return _fileDtos;
    }
}