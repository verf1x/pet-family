using Microsoft.AspNetCore.Mvc;
using PetFamily.Api.Response;

namespace PetFamily.Api.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : ControllerBase { }