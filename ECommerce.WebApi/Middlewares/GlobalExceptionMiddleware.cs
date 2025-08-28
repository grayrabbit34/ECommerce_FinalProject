using System;

using System.Net;
using System.Text.Json;

namespace ECommerce.WebApi.Middlewares
{
    /// Tüm beklenmeyen hataları yakalayıp tek tip JSON dönen global hata yakalayıcı.
   
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Basitçe consola yaz. Üretimde structured logging tercih edilir.
                Console.WriteLine($"[ERROR] {ex}");

                // Standart 500 cevabı
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(new { message = "Beklenmeyen bir hata oluştu." });
                await context.Response.WriteAsync(payload);
            }
        }
    }

}

