namespace API.Filters;

public static class AuthenticationFilterExtension
{
    public static RouteHandlerBuilder RequireAuthenticated(this RouteHandlerBuilder routeHandlerBuilder)
    {
        routeHandlerBuilder.AddEndpointFilter<RequireAuthenticatedFilter>();

        return routeHandlerBuilder;
    }
}