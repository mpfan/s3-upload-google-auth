using API.Services;
using API.Messaging.Requests;
using MediatR;

namespace API.Messaging.Handlers;

public class ListFileNamesHandler : IRequestHandler<ListFileNamesRequest, IEnumerable<string>>
{
    private readonly IFileStorageService _storageService;

    public ListFileNamesHandler(IFileStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<IEnumerable<string>> Handle(ListFileNamesRequest request, CancellationToken cancellationToken)
    {
        return await _storageService.GetFileNames(request.BucketName);
    }
}