using System.Diagnostics;
using Pawfectmatch_V._1.Services;

namespace Pawfectmatch_V._1.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogService _logService;

        public RequestLoggingMiddleware(RequestDelegate next, ILogService logService)
        {
            _next = next;
            _logService = logService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var method = context.Request.Method;
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            try
            {
                _logService.LogInformation($"Request started: {method} {requestPath} from {ipAddress}");

                await _next(context);

                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;

                if (statusCode >= 400)
                {
                    _logService.LogWarning($"Request completed with status {statusCode}: {method} {requestPath} in {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    _logService.LogInformation($"Request completed successfully: {method} {requestPath} in {stopwatch.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logService.LogError($"Request failed: {method} {requestPath} in {stopwatch.ElapsedMilliseconds}ms", ex);
                throw;
            }
        }
    }

    // Extension method for easy middleware registration
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
} 