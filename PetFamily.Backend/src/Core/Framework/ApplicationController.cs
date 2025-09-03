using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework.Response;

namespace PetFamily.Framework;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);

        return base.Ok(envelope);
    }
}