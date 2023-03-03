using Amazon.S3.Model;
using API.S3;
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

builder.Services.AddS3ClientFactory();

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

app.MapGet("/api/files", async (HttpContext httpContext, IConfiguration configuration, IS3ClientFactory s3ClientFactory) =>
{
    var client = s3ClientFactory.GetClient();
    var files = new List<string>();

    var listObjectRequest = new ListObjectsRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"]

    };

    ListObjectsResponse listObjectResponse;

    do
    {
        listObjectResponse = await client.ListObjectsAsync(listObjectRequest);

        foreach (var obj in listObjectResponse.S3Objects)
        {
            files.Add(obj.Key);
        }

    } while (listObjectResponse.IsTruncated);

    return Results.Ok(files);
});

app.MapGet("api/files/{file}", async (string file, HttpContext httpContext, IConfiguration configuration, IS3ClientFactory s3ClientFactory) => {
    var client = s3ClientFactory.GetClient();
    
    var getObjectRequest = new GetObjectRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"],
        Key = file
    };

    var getObjectResponse = await client.GetObjectAsync(getObjectRequest);

    return Results.File(getObjectResponse.ResponseStream);
});

app.MapPost("/api/files", async (HttpContext httpContext, IConfiguration configuration, IFormFile file, IS3ClientFactory s3ClientFactory) =>
{
    var client = s3ClientFactory.GetClient();

    var putObjectRequest = new PutObjectRequest
    {
        BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"],
        Key = Path.GetFileName(file.FileName),
        InputStream = file.OpenReadStream(),
    };

    await client.PutObjectAsync(putObjectRequest);

    return Results.Ok(Path.GetFileName(file.FileName));
});

app.Run();
