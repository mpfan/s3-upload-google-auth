using Serilog;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using API.Infrastructure;
using API.Messaging.Requests;
using API.Behavirous;
using API.Validators;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["DOTNET_CLIENT_ID"];
    options.ClientSecret = builder.Configuration["DOTNET_CLIENT_SECRET"];
    options.CallbackPath = new PathString(builder.Configuration["DOTNET_CALLBACK_PATH"]);
})
.AddCookie();

builder.Services.AddMediatR
(
    cfg => cfg
    .RegisterServicesFromAssembly(typeof(Program).Assembly)
    .AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>))
    .AddOpenBehavior(typeof(ValidatorPipelineBehaviour<,>))
);
builder.Services.AddScoped<IValidator<PutFileRequest>, PutFileRequestValidator>();

builder.Services.AddS3FileStorage();

var app = builder.Build();

app.UseExceptionHandler("/Error");

app.Map("/Error", (HttpContext httpContext) =>
{
    var exceptionHandlerPathFeature =
            httpContext.Features.Get<IExceptionHandlerPathFeature>();

    var error = exceptionHandlerPathFeature?.Error;

    if (error is ValidationException)
    {
        return Results.BadRequest(error.Message);
    }

    return Results.Problem();
});

app.UseAuthentication();

app.Use(async (HttpContext httpContext, RequestDelegate next) =>
{
    var IsAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;

    if (!IsAuthenticated && !(httpContext.Request.Path == "/api/login"))
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        return;
    }

    await next(httpContext);
});

app.MapGet("/api/login", (HttpContext httpContext, IConfiguration configuration) =>
{
    return Results.Challenge(new AuthenticationProperties { RedirectUri = configuration["DOTNET_CLIENT_URL"] });
});

app.MapGet("/api/files", async (HttpContext httpContext, IConfiguration configuration, IMediator mediator) =>
{
    var request = new ListFileNamesRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"]
    };

    var files = await mediator.Send(request);

    return Results.Ok(files);
});

app.MapGet("api/files/{file}", async (string file, HttpContext httpContext, IConfiguration configuration, IMediator mediator) =>
{
    var request = new GetFileRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"],
        Key = file
    };

    var fileStream = await mediator.Send(request);

    return Results.File(fileStream);
});

app.MapPost("/api/files", async (HttpContext httpContext, IConfiguration configuration, IFormFile file, IMediator mediator) =>
{
    var request = new PutFileRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"],
        Key = "",
        File = file.OpenReadStream()
    };

    await mediator.Send(request);

    return Results.Ok(Path.GetFileName(file.FileName));
});

app.Run();
