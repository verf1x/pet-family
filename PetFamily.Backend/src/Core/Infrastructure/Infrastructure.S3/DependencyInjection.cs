using Infrastructure.S3.BackgroundServices;
using Infrastructure.S3.MessageQueues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PetFamily.Framework.Files;
using PetFamily.Framework.Messaging;
using PetFamily.Infrastructure.Options;

namespace Infrastructure.S3;

public static class DependencyInjection
{
    public static IServiceCollection AddS3Infrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddServices()
            .AddMessageQueues()
            .AddHostedServices()
            .AddMinio(configuration);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFilesCleanerService, FilesCleanerService>();

        return services;
    }

    private static IServiceCollection AddMessageQueues(this IServiceCollection services)
    {
        services.AddSingleton<IMessageQueue<IEnumerable<string>>, InMemoryMessageQueue<IEnumerable<string>>>();

        return services;
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<FilesCleanerBackgroundService>();

        return services;
    }

    private static IServiceCollection AddMinio(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MinioOptions>(
            configuration.GetSection(MinioOptions.Minio));

        services.AddMinio(options =>
        {
            var minioOptions = configuration
                .GetSection(MinioOptions.Minio)
                .Get<MinioOptions>() ?? throw new ApplicationException("Minio options not found");

            options.WithEndpoint(minioOptions.Endpoint);

            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

            options.WithSSL(minioOptions.WithSsl);
        });

        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }
}