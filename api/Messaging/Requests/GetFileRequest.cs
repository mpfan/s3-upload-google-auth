using MediatR;

namespace API.Messaging.Requests;

public sealed record GetFileRequest : IRequest<Stream>
{
    public string BucketName { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
}