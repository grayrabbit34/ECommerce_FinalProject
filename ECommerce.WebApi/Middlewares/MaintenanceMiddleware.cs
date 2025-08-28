using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Business.Operations.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection; // GetRequiredService için

namespace ECommerce.WebApi.Middlewares
{
    /// <summary>
    /// Sistem bakım modundayken (Settings.MaintenanceMode = true)
    /// beyaz listedeki yollar hariç tüm istekleri 503 ile keser.
    /// </summary>
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        // Tam eşleşen izinli yollar
        private static readonly HashSet<string> AllowedExact = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/settings/maintenance-state",
            "/api/settings/maintenance-toggle"
        };

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Swagger veya beyaz listedekiler → kontrolsüz geç
            if (AllowedExact.Contains(path) ||
                path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // ISettingService scoped → her istekte RequestServices üzerinden çöz
            var settingService = context.RequestServices.GetRequiredService<ISettingService>();
            bool maintenanceMode = await settingService.GetMaintenanceState();

            if (maintenanceMode)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Sistem bakım modunda. Lütfen daha sonra tekrar deneyiniz.");
                return; // pipeline'ı durdur
            }

            await _next(context);
        }
    }

}
