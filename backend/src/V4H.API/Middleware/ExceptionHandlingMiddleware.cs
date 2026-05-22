using System.Text.Json;
using V4H.Application.Common.Exceptions;

namespace V4H.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, extra) = exception switch
        {
            NotFoundException nfe => (StatusCodes.Status404NotFound, nfe.Message, (object?)null),
            UnauthorizedException ue => (StatusCodes.Status403Forbidden, ue.Message, null),
            InvalidOperationException ioe => (StatusCodes.Status409Conflict, ioe.Message, null),
            DocumentValidationException dve => (StatusCodes.Status422UnprocessableEntity, dve.Message,
                (object?)new { score = dve.Score }),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", (object?)null)
        };

        context.Response.StatusCode = statusCode;

        var body = extra is not null
            ? JsonSerializer.Serialize(new { error = message, extra })
            : JsonSerializer.Serialize(new { error = message });

        return context.Response.WriteAsync(body);
    }
}
