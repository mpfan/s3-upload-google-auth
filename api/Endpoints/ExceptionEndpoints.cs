using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Endpoints;

public static class ExceptionEndpoints
{
    public static void MapExceptionEndpoints(this IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.Map("/Error", HandleException);
    }

    public static IResult HandleException(HttpContext httpContext)
    {
        var exceptionHandlerPathFeature =
            httpContext.Features.Get<IExceptionHandlerPathFeature>();

        var error = exceptionHandlerPathFeature?.Error;

        if (error is ValidationException)
        {
            return Results.BadRequest(error.Message);
        }

        return Results.Problem();
    }
}