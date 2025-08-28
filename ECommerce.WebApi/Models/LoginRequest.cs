using System;

using System.ComponentModel.DataAnnotations;


namespace ECommerce.WebApi.Models
{
    /// Giriş (login) isteği modelidir. Kullanıcı adı/şifre taşır.
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

}

