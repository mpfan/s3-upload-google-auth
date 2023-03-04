using MediatR;

namespace API.Messaging.Requests;

public sealed record PutFileRequest : IRequest
{
    public string BucketName { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;

    public Stream File { get; init; } = Stream.Null;
}