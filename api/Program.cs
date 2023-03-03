using API.Abstractions;
using API.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddS3FileStorage();

var app = builder.Build();

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

app.MapGet("/api/files", async (HttpContext httpContext, IConfiguration configuration, IFileStorageService storageService) =>
{
    var files = await storageService.GetFileNames(configuration["DOTNET_MINIO_BUCKET_NAME"]);

    return Results.Ok(files);
});

app.MapGet("api/files/{file}", async (string file, HttpContext httpContext, IConfiguration configuration, IFileStorageService storageService) => {
    var fileStream = await storageService.GetFile(configuration["DOTNET_MINIO_BUCKET_NAME"], file);

    return Results.File(fileStream);
});

app.MapPost("/api/files", async (HttpContext httpContext, IConfiguration configuration, IFormFile file, IFileStorageService storageService) =>
{
    await storageService.PutFile(configuration["DOTNET_MINIO_BUCKET_NAME"], Path.GetFileName(file.FileName), file.OpenReadStream());

    return Results.Ok(Path.GetFileName(file.FileName));
});

app.Run();
