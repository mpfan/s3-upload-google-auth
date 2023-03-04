using MediatR;

namespace API.Behavirous;

public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling @{Request} at @{Time}", request, DateTime.UtcNow);
        var response = await next();
        _logger.LogInformation("{RequestName} handled at @{Time}", typeof(TRequest).Name, DateTime.UtcNow);

        return response;
    }
}