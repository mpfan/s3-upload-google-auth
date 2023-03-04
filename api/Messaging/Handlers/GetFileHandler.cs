using API.Services;
using API.Messaging.Requests;
using MediatR;

namespace API.Messaging.Handlers;

public class GetFileHandler : IRequestHandler<GetFileRequest, Stream>
{
    private readonly IFileStorageService _storageService;

    public GetFileHandler(IFileStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<Stream> Handle(GetFileRequest request, CancellationToken cancellationToken)
    {
        return await _storageService.GetFile(request.BucketName, request.Key);
    }
}