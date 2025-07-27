using MediatR;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger _logger = Log.ForContext<LoggingBehavior<TRequest, TResponse>>();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.Information("Processing request of type: {requestType}", typeof(TRequest).Name);

        var response = await next(ct);

        _logger.Information("Request handling completed with response type: {responseType}", typeof(TResponse).Name);

        return response;
    }
}