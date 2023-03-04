using API.Filters;
using API.Messaging.Requests;
using MediatR;

namespace API.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder endPointBuilder)
    {
        endPointBuilder.MapGet("api/files", GetFileNames).RequireAuthenticated();
        endPointBuilder.MapGet("api/files/{file}", GetFile).RequireAuthenticated();
        endPointBuilder.MapPost("api/files", PutFile).RequireAuthenticated();
    }

    public static async Task<IResult> GetFile(string file, IConfiguration configuration, IMediator mediator)
    {
        var request = new GetFileRequest
        {
            BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"] ?? throw new Exception("No bucket name set in configuration"),
            Key = file
        };

        var fileStream = await mediator.Send(request);

        return Results.File(fileStream);
    }

    public static async Task<IResult> GetFileNames(IConfiguration configuration, IMediator mediator)
    {
        var request = new ListFileNamesRequest
        {
            BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"] ?? throw new Exception("No bucket name set in configuration")
        };

        var files = await mediator.Send(request);

        return Results.Ok(files);
    }

    public static async Task<IResult> PutFile(IFormFile file, IConfiguration configuration, IMediator mediator)
    {
        var request = new PutFileRequest
        {
            BucketName = configuration["DOTNET_MINIO_BUCKET_NAME"] ?? throw new Exception("No bucket name set in configuration"),
            Key = Path.GetFileName(file.FileName),
            File = file.OpenReadStream()
        };

        await mediator.Send(request);

        return Results.Ok(Path.GetFileName(file.FileName));
    }
}