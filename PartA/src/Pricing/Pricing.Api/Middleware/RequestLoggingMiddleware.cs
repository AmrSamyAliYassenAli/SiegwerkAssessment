namespace Pricing.Api.Middleware;

/// <summary>
/// Middleware to log incoming HTTP requests and outgoing responses.
/// It records details such as method, path, status code, and duration,
/// and adds a correlation ID for tracing requests across services.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger for this middleware.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to log request and response details.
    /// It measures the duration of the request and logs key information at various stages.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        // Add correlation ID to response headers
        context.Response?.Headers?.Append("X-Correlation-ID", correlationId);

        // Log request start
        _logger.LogInformation(
            "Request started: {Method} {Path} from {RemoteIpAddress} with TraceId: {TraceId} and CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            traceId,
            correlationId);

        // Add request body logging for non-GET requests (be careful with sensitive data)
        if (context.Request.Method != "GET" && context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            // Only log body for small requests to avoid performance issues
            if (body.Length < 1000)
            {
                _logger.LogDebug("Request body: {RequestBody}", body);
            }
        }

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log request completion
            _logger.LogInformation(
                "Request completed: {Method} {Path} with status {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}, CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response!.StatusCode,
                stopwatch.ElapsedMilliseconds,
                traceId,
                correlationId);

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 5000)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    traceId);
            }
        }
    }
}
