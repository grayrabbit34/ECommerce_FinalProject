using System;
namespace ECommerce.WebApi.Models
{

    /// Başarılı giriş sonrası istemciye döndüğümüz yanıt modeli.
    /// Token ve sona erme zamanı verilir.
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Message { get; set; }
    }


}

