using System;
namespace ECommerce.WebApi.Middlewares
{

    /// Tüm HTTP isteklerini basitçe konsola loglayan middleware.
    /// Üretimde gerçek bir logger (Serilog/NLog) tercih edilir.

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            // Token varsa kullanıcı Id claim'ini al; yoksa '-'
            var userId = context.User?.FindFirst("Id")?.Value ?? "-";

            Console.WriteLine($"[{DateTime.UtcNow:O}] {context.Request.Method} {context.Request.Path} (User:{userId})");

            await _next(context);
        }
    }

}

