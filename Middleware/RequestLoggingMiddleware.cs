using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Copilot debugging utilization: Added a try-catch block here to ensure that 
            // any unhandled exceptions during request processing still allow logging of the failure,
            // preventing the middleware from silently swallowing context state.
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var method = context.Request.Method;
                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;
                var executionTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ExecutionTime} ms",
                    method, path, statusCode, executionTimeMs);
            }
        }
    }
}
