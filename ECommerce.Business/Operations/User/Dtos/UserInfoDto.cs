using System;
using ECommerce.Data.Enums;

namespace ECommerce.Business.Operations.User.Dtos
{
 
    //giriş işlemi başarıyla tamamlanmışsa token oluşturmak için kullanacağım dto
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Role Role { get; set; }
    }

}

