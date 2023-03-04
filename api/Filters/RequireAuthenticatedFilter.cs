namespace API.Filters;

public class RequireAuthenticatedFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var IsAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}