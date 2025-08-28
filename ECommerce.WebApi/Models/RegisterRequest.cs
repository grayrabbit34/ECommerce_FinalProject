using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApi.Models
{        
    
    /// Kullanıcı kayıt isteği modelidir. Kullanıcı kayıt olurken hangi verilerin isteneceği belirleriz.
    /// Validation attributeleri ile model doğrulama yapılır.

    public class RegisterRequest
    {
        // Zorunlu + e-posta format kontrolü
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        // Zorunlu + minimum uzunluk
        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required ]
        public string PhoneNumber { get; set; } = null!;
    }

}

