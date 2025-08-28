using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ECommerce.Business.Operations.User;
using ECommerce.Business.Operations.User.Dtos;
using ECommerce.WebApi.Jwt;
using ECommerce.WebApi.Models;

using Microsoft.AspNetCore.Authorization;


/// <summary>


namespace ECommerce.WebApi.Controllers
{

    /// Kayıt ve giriş işlemlerini yöneten controller.

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _cfg;


        //dependency injection ile _userService sınıfından metotdları kullanabilirim
    
        public AuthController(IUserService userService, IConfiguration cfg)
        {
            _userService = userService;
            _cfg = cfg;
        }

        /// <summary>
        /// Yeni kullanıcı kaydı (anonymous erişim).
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register( RegisterRequest request)
        {
            // Model doğrulaması (DataAnnotation) – otomatik 400 dönmez, biz kontrol ediyoruz
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //  DI ile mümkün hale geitrdiğim adduser metodunu koullanıcam
            //veri istediğim tipteyse Dto tipinde bi nesneye atıyorum verileri 
            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };


            var result = await _userService.AddUser(addUserDto);

            if (!result.IsSucceed)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Kullanıcı girişi – başarılı ise JWT üretip döner.
        /// verinin Url üzerinde gözükmemesi için ve url'ler loglanacağı için şifreler loglanmasın diye HTTPGet yerine post kullanıyorum
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest request)
        {
            //api valid mi kontrol ediyorum
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //clienttan gelen istekteki bilgileri dtoya gönderiyorum
            //yani swaggerdan  oturum açma için gelen kullanıcı bilgilerini dtoya gönderiyorum
            //single responsibility
            var result = _userService.LoginUser(new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password
            });

            if (!result.IsSucceed || result.Data == null)
                return Unauthorized("Kullanıcı veya şifre hatalı.");

            var user1 = result.Data;

            //tüm controller genelinde kullanmama gerek olmayan bi DI yapacağımız için construtor injectiona gerek yok
            //request response yaşam döngüm içinde tüm bilgileri tutan HttpContext'i kullanıyorum
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user1.Id,
                Email = user1.Email,
                FirstName = user1.FirstName,
                LastName = user1.LastName,
                Role = user1.Role,
                SecretKey = configuration["Jwt:SecretKey"]!,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
            });

            return Ok( new LoginResponse
            {
                Message = "Giriş başarıyla tamamlandı",
                Token = token
            });           
        }

        /// <summary>
        /// Basit profil – token içindeki claim'leri geri döner.
        /// </summary>
        [HttpGet("me")]
        [Authorize] // token yoksa cevap dönmesin
        public IActionResult GetMyUser()
        {
            var id = User.FindFirst(JwtClaimNames.Id)?.Value;
            var first = User.FindFirst(JwtClaimNames.FirstName)?.Value;
            var last = User.FindFirst(JwtClaimNames.LastName)?.Value;
            var role = User.FindFirst(JwtClaimNames.Role)?.Value;

            return Ok(new { id, first, last, role });
        }
    }

}

