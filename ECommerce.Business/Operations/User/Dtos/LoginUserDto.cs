using System;
namespace ECommerce.Business.Operations.User.Dtos
{
    /// <summary>
    /// giriş işlemlerinde kullanacağım dtoları içieriyor
    /// </summary>
    public class LoginUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}

