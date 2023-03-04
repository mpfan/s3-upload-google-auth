using MediatR;

namespace API.Messaging.Requests;

public sealed record ListFileNamesRequest : IRequest<IEnumerable<string>>
{
    public string BucketName { get; init; } = string.Empty;
}