using System;
using Microsoft.AspNetCore.Builder;

namespace ECommerce.WebApi.Middlewares
{      
    /// Middleware'i app pipeline'ına eklemek için kısa yol extension.
    /// Kullanım: app.UseRequestLogging();

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}

