using System;

using ECommerce.Business.Operations.User.Dtos;
using ECommerce.Business.Types;

namespace ECommerce.Business.Operations.User
{

    //types klasöründeki servicemessage tipinde veri döneceğim
    public interface IUserService
    {

        //async çünkü unit of work kullanacağım

        Task<ServiceMessage> AddUser(AddUserDto user);


        ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user);
    }

}



