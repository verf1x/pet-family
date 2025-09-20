using Accounts.Application.Authorization;
using Accounts.Domain;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel;

namespace Accounts.Application.AccountsManagement.Commands.Login;

public class LoginHandler : ICommandHandler<string, LoginCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        UserManager<User> userManager,
        ITokenProvider tokenProvider,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Errors.General.NotFound().ToErrorList();

        bool passwordConfirmed = await _userManager.CheckPasswordAsync(user, command.Password);
        if (!passwordConfirmed)
            return Errors.User.InvalidCredentials().ToErrorList();

        string token = _tokenProvider.GenerateAccessToken(user);

        _logger.LogInformation("User '{userId}' logged in.", user.Id);

        return token;
    }
}