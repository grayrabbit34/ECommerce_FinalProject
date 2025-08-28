using System;
using Microsoft.AspNetCore.Builder;

namespace ECommerce.WebApi.Middlewares
{
    using Microsoft.AspNetCore.Builder;

  
    /// app.UseGlobalException(); ile kolay ekleme.
    /// </summary>
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {

            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
          
    }

}

