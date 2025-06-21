using Microsoft.AspNetCore.Mvc;

namespace PetFamily.Api.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase { }