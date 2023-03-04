using API.Services;
using API.Messaging.Requests;
using MediatR;

namespace API.Messaging.Handlers;

public class PutFileHandler : IRequestHandler<PutFileRequest>
{
    private readonly IFileStorageService _fileStorageService;

    public PutFileHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(PutFileRequest request, CancellationToken cancellationToken)
    {
        await _fileStorageService.PutFile(request.BucketName, request.Key, request.File);
    }
}