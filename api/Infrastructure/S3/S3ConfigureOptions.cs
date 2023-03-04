using System.Net;
using Microsoft.Extensions.Options;

namespace API.Infrastructure.S3;

public class S3ConfigureClientOptions : IConfigureOptions<S3ClientOptions>, IValidateOptions<S3ClientOptions>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<S3ConfigureClientOptions> _logger;

    public S3ConfigureClientOptions(IConfiguration configuration, ILogger<S3ConfigureClientOptions> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void Configure(S3ClientOptions options)
    {
        options.AccessKey = _configuration["DOTNET_MINIO_ACCESS_KEY"];
        options.AccessSecret = _configuration["DOTNET_MINIO_SECRET_KEY"];

        // Need to resolve to ip to fix service not found
        var originalServiceUrl = _configuration["DOTNET_MINIO_SERVICE_URL"];
        var serviceURL = GetServiceIP(_configuration["DOTNET_MINIO_SERVICE_URL"]);

        _logger.LogInformation("{originalServiceUrl} resolved to {serviceUrl}", originalServiceUrl, serviceURL);

        options.ServiceUrl = serviceURL;
    }

    public ValidateOptionsResult Validate(string? name, S3ClientOptions options)
    {
        var failMessages = new List<string>();
        
        if (string.IsNullOrEmpty(options.ServiceUrl))
        {
            failMessages.Add("ServiceUrl not set");
        }
        if (string.IsNullOrEmpty(options.AccessKey))
        {
            failMessages.Add("AccessKey not set");
        }
        if (string.IsNullOrEmpty(options.AccessSecret))
        {
            failMessages.Add("AccessSecret not set");
        }

        if(failMessages.Count > 0) {
            return ValidateOptionsResult.Fail(failMessages);
        }

        return ValidateOptionsResult.Success;
    }

    private static string GetServiceIP(string orginalServiceURL)
    {
        var originalUri = new Uri(orginalServiceURL);

        var hostEntry = Dns.GetHostEntry(originalUri.DnsSafeHost);
        var ip = hostEntry.AddressList[0];

        var newUriBuilder = new UriBuilder();

        newUriBuilder.Scheme = originalUri.Scheme;
        newUriBuilder.Host = ip.ToString();
        newUriBuilder.Port = originalUri.Port;

        return newUriBuilder.Uri.ToString();
    }
}