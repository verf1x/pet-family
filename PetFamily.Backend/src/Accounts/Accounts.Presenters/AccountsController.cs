using Accounts.Application.AccountsManagement.Commands.Login;
using Accounts.Application.AccountsManagement.Commands.Register;
using Accounts.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.ResponseExtensions;

namespace Accounts.Presenters;

public class AccountsController : ApplicationController
{
    [HttpPost("registration")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.UserName,
            request.Password);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginUserRequest request,
        [FromServices] LoginHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}