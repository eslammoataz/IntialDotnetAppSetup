using System.Diagnostics;
using Serilog.Context;

namespace TestingProjectSetup.Api.Middleware;

/// <summary>
/// Middleware to log HTTP request and response details including path, method, status code, and duration.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var request = context.Request;

        try
        {
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

            using (LogContext.PushProperty("UserId", userId))
            {
                await _next(context);
            }

            sw.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms",
                request.Method,
                request.Path,
                context.Response.StatusCode,
                sw.Elapsed.TotalMilliseconds);
        }
        catch (Exception)
        {
            sw.Stop();
            _logger.LogWarning(
                "HTTP {Method} {Path} failed in {Elapsed:0.0000} ms",
                request.Method,
                request.Path,
                sw.Elapsed.TotalMilliseconds);
            throw;
        }
    }
}
