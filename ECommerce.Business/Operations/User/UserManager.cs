using System;
using ECommerce.Business.DataProtection;
using ECommerce.Business.Operations.User.Dtos;
using ECommerce.Business.Types;
using ECommerce.Data.Entities;
using ECommerce.Data.Enums;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWork;


namespace ECommerce.Business.Operations.User
{
 

    public class UserManager : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtection _dataProtection;

        public UserManager(IRepository<UserEntity> userRepository,
                           IUnitOfWork unitOfWork,
                           IDataProtection dataProtection)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _dataProtection = dataProtection;
        }

        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            // Benzersiz mail kontrolü
            var hasEmail = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower());
            if (hasEmail != null)
            {
                return new ServiceMessage { IsSucceed = false, Message = "Email adresi zaten mevcut." };
            }
            //dto tipindeki nesnenin verilerini UserEntity tipinde nesneye aktarıyorum
            var entity = new UserEntity
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Password = _dataProtection.Protect(user.Password),
                Role = Role.Customer
            };

            _userRepository.Add(entity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception) 
            {
                throw new Exception("Kullanıcı kaydı sırasında bir hata oluştu");
            }
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Kayıt başarılı."
            };
        }

        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {
            //form/swagger üzerinden gönderilen kullanıcı giriş bilgileri sistemdeki herhangi bir kullanıcıyla eşleşiyor mu kontrol ediliyor
            var userEntity = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower());

            //girilen kullanıcı emaili veritabanından herhangi bir kullanıcıyla emailiyle eşleşmediyse
            if (userEntity is null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı adı veya şifre hatalı"
                };
            }
            //veritabanından gelen kullanıcı şifresi userEntity isimli UserEntity tipindeki nesnenin içinde
            //şifre protected şekilde tutulduğu için unprotected metoduyla asıl şifreyi plainpassword değişkenine atıyroum
            var plainPassword = _dataProtection.UnProtect(userEntity.Password);

            //kullanıcının girdiği şifre sistemdeki şifreyle uyuşmuyorsa
            if (plainPassword != user.Password)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı adı veya şifre hatalı"
                };
            }

            return new ServiceMessage<UserInfoDto>
            {
                IsSucceed = true,
                Data = new UserInfoDto
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Role = userEntity.Role
                }
            };
        }
    }

}

