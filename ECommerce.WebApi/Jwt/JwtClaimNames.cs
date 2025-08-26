using System;
namespace ECommerce.WebApi.Jwt
{

    /// Token içine yazdığımız custom claim adlarını sabitlemek için tek bir yerden yönetilen anahtar isimleri.
    public class JwtClaimNames
    {
        public const string Id = "Id";               // Kullanıcı Id’si
        public const string Email = "Email";         // kullanıcı Emaili
        public const string FirstName = "FirstName"; // Ad
        public const string LastName = "LastName";   // Soyad
        public const string Role = "Role";           // Rol (Admin/Customer)
    }

}

