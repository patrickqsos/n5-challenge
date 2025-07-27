using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using N5Challenge.Exceptions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Middlewares;

 public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger = Log.ForContext<ExceptionHandlingMiddleware>();

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problem;
        HttpStatusCode statusCode;

        switch (ex)
        {
            /*
            case ValidationException ve:
                statusCode = HttpStatusCode.BadRequest;
                problem = new ValidationProblemDetails(ve.Errors)
                {
                    Title = ve.Message,
                    Status = (int)statusCode,
                };
                break;
*/
            case NotFoundException nfe:
                statusCode = HttpStatusCode.NotFound;
                problem = new ProblemDetails
                {
                    Title = nfe.Message,
                    Status = (int)statusCode,
                };
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = (int)statusCode,
                };
                _logger.Error(ex, "Unhandled exception");
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problem);
        return context.Response.WriteAsync(json);
    }
}