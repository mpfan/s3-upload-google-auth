namespace API.S3;

public static class S3ClientExtensions
{
    public static IServiceCollection AddS3ClientFactory(this IServiceCollection services)
    {
        services.AddOptions<S3ClientOptions>().ValidateOnStart();
        services.ConfigureOptions<S3ConfigureClientOptions>();

        services.AddSingleton<IS3ClientFactory, S3ClientFactory>();

        return services;
    }
} 