using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.WebApi.Filters
{
   

    
    /// Belirli saat aralıkları dışında endpoint'i otomatik 403 kapatan Action Filter.
    /// Örn: Ürün güncelleme yalnızca 15:00-23:59 arası.
    /// 
    public class TimeControlFilter : ActionFilterAttribute
    {
        // Europe/Istanbul yerel saati baz alınır (sunucu lokal saatine göre)
        public string StartTime { get; set; } = "15:00";
        public string EndTime { get; set; } = "23:59";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var now = DateTime.Now.TimeOfDay;
            var start = TimeSpan.Parse(StartTime);
            var end = TimeSpan.Parse(EndTime);

            // Aralık dışındaysa 403 yanıtı üret
            if (now < start || now > end)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Bu endpointe yalnızca belirlenen saatlerde erişilebilir."
                };
            }

            base.OnActionExecuting(context);
        }
    }

}

