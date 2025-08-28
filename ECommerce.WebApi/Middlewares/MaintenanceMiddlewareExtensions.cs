using System;
using Microsoft.AspNetCore.Builder;

namespace ECommerce.WebApi.Middlewares
{
    /// app.UseMaintenanceMode(); ile kolay ekleme.
    /// </summary>
    public static class MaintenanceMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder app)
            => app.UseMiddleware<MaintenanceMiddleware>();
    }

}

