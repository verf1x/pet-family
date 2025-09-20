using PetFamily.Core.Abstractions;

namespace Accounts.Application.AccountsManagement.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand;