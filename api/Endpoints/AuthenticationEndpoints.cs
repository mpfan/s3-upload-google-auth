using Microsoft.AspNetCore.Authentication;

namespace API.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapGet("api/login", Login);
    }

    public static IResult Login(IConfiguration configuration) =>
        Results.Challenge(new AuthenticationProperties { RedirectUri = configuration["DOTNET_CLIENT_URL"] });
}