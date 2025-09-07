namespace Pricing.Api.Middleware;

/// <summary>
/// A global exception handling middleware that catches unhandled exceptions in the request pipeline,
/// logs them, and returns a standardized, RFC 7807-compliant problem details response to the client.
/// This middleware provides a centralized way to handle errors consistently across the API.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger for this middleware.</param>
    /// <param name="environment">The current hosting environment.</param>
    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP request. It uses a try-catch block to
    /// intercept any unhandled exceptions and pass them to the exception handler.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. TraceId: {TraceId}", traceId);
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    /// <summary>
    /// Handles the caught exception by setting the appropriate HTTP status code and
    /// returning a detailed JSON response based on the exception type.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that was caught.</param>
    /// <param name="traceId">The unique trace ID for the request.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        context.Response.ContentType = "application/json";

        var response = new GlobalErrorResponse
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            Error = new ErrorDetails
            {
                TraceId = traceId,
                Instance = context.Request.Path
            }
        };

        switch (exception)
        {
            case ArgumentException argEx:
                response.Error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                response.Error.Title = "Bad Request";
                response.Error.Status = (int)HttpStatusCode.BadRequest;
                response.Error.Detail = argEx.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                response.Error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                response.Error.Title = "Not Found";
                response.Error.Status = (int)HttpStatusCode.NotFound;
                response.Error.Detail = "The requested resource was not found";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case TimeoutException:
                response.Error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.7";
                response.Error.Title = "Request Timeout";
                response.Error.Status = (int)HttpStatusCode.RequestTimeout;
                response.Error.Detail = "The request timed out";
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            case NotImplementedException:
                response.Error.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                response.Error.Title = "Not Implemented";
                response.Error.Status = (int)HttpStatusCode.NotImplemented;
                response.Error.Detail = "This feature is not implemented";
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                break;

            default:
                response.Error.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                response.Error.Title = "Internal Server Error";
                response.Error.Status = (int)HttpStatusCode.InternalServerError;
                response.Error.Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request";

                if (_environment.IsDevelopment())
                {
                    response.Error.Extensions["StackTrace"] = exception.StackTrace!;
                    response.Error.Extensions["InnerException"] = exception.InnerException?.Message!;
                }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}