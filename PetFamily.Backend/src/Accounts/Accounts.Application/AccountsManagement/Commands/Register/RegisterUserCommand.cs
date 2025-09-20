using PetFamily.Core.Abstractions;

namespace Accounts.Application.AccountsManagement.Commands.Register;

public record RegisterUserCommand(string Email, string UserName, string Password) : ICommand;