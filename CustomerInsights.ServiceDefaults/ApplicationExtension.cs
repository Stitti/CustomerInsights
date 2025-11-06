using CustomerInsights.ServiceDefaults.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace CustomerInsights.ServiceDefaults;

public static class ApplicationExtension
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }

    public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}