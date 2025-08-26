using System;

using ECommerce.Data.Enums;
namespace ECommerce.WebApi.Jwt
{
    

    /// Token üretiminde ihtiyaç duyduğumuz kullanıcı ve konfig bilgilerini
    /// tek modelde toplar. WebApi -> JwtHelper'a gider.
    // güvenlik açığı olmamaması için burada password açmıyoruz
    public class JwtDto
    {
        // Kullanıcı bilgileri (token claim'lerine yazılacak)
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Role Role { get; set; }

        // Konfigürasyon (appsettings.json'dan okunur)
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpireMinutes { get; set; } = 60; // Varsayılan 60 dk
    }

}

