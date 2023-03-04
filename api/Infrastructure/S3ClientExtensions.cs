using API.Services;
using API.Infrastructure.S3;

namespace API.Infrastructure;

public static class S3ClientExtensions
{
    public static IServiceCollection AddS3FileStorage(this IServiceCollection services)
    {
        services.AddOptions<S3ClientOptions>().ValidateOnStart();
        services.ConfigureOptions<S3ConfigureClientOptions>();

        services.AddSingleton<IS3ClientFactory, S3ClientFactory>();
        services.AddTransient<IFileStorageService, S3FileStorageService>();

        return services;
    }
} 