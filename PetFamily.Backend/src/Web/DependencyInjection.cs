using Accounts.Presenters;
using Microsoft.OpenApi.Models;
using Species.Presenters;
using Volunteers.Presenters;

namespace Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddWebDependencies();

        services.AddVolunteersModule(configuration);

        services.AddSpeciesModule(configuration);

        services.AddAccountsModule(configuration);

        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pet Family API", Version = "v1" });
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    },
                    []
                },
            });
        });


        return services;
    }
}