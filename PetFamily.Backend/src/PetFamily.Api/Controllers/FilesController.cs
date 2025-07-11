using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Extensions;
using PetFamily.Application.Files.GetPresignedUrl;
using PetFamily.Application.Files.Remove;
using PetFamily.Application.Files.Upload;

namespace PetFamily.Api.Controllers;

public class FilesController : ApplicationController
{
    // [HttpPost("upload")]
    // public async Task<IActionResult> UploadFileAsync(
    //     IFormFile file,
    //     [FromServices] UploadFileHandler handler,
    //     CancellationToken cancellationToken = default)
    // {
    //     await using var fileContent = file.OpenReadStream();
    //     
    //     var fileData = new UploadFileRequest(
    //         fileContent,
    //         "photos",
    //         Guid.NewGuid().ToString());
    //     
    //     var result = await handler.HandleAsync(fileData, cancellationToken);
    //     if (result.IsFailure)
    //         return result.Error.ToResponse();
    //     
    //     return Ok(result.Value);
    // }
    
    [HttpDelete("remove/{bucket}/{file}")]
    public async Task<IActionResult> RemoveFileAsync(
        [FromRoute] string bucket,
        [FromRoute] string file,
        [FromServices] RemoveFileHandler handler,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveFileRequest(bucket, file);
        
        var result = await handler.HandleAsync(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
    
    [HttpGet("presigned-url/{bucket}/{file}")]
    public async Task<IActionResult> GetPresignedUrlAsync(
        [FromRoute] string bucket,
        [FromRoute] string file,
        [FromServices] GetPresignedUrlHandler handler,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPresignedUrlRequest(bucket, file, TimeSpan.FromMinutes(15));
        
        var result = await handler.HandleAsync(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();
        
        return Ok(result.Value);
    }
    
}