using Accounts.Application.AccountsManagement.Commands;
using Accounts.Application.AccountsManagement.Commands.Login;
using Accounts.Application.AccountsManagement.Commands.Register;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserHandler>();

        services.AddScoped<LoginHandler>();

        return services;
    }
}