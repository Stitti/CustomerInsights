using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CustomerInsights.ServiceDefaults.Middlewares
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string traceId = Guid.NewGuid().ToString();

            context.Items["TraceId"] = traceId;
            context.Response.Headers["X-Trace-Id"] = traceId;

            using (LogContext.PushProperty("TraceId", traceId))
            using (_logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = traceId }))
            {
                string method = context.Request.Method;
                string path = context.Request.Path;
                Stopwatch stopwatch = Stopwatch.StartNew();

                _logger.LogInformation("Incoming request: {Method} {Path}", method, path);

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception for {Method} {Path}", method, path);
                    throw;
                }

                stopwatch.Stop();

                int statusCode = context.Response.StatusCode;
                LogLevel level = statusCode switch
                {
                    >= 500 => LogLevel.Error,
                    >= 400 => LogLevel.Warning,
                    _ => LogLevel.Information
                };

                _logger.Log(level, "Completed {Method} {Path} → {StatusCode} in {Elapsed}ms", method, path, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}