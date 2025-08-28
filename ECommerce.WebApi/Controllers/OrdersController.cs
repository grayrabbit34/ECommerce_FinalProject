using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ECommerce.Business.Operations.Order;
using ECommerce.Business.Operations.Order.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.WebApi.Models;
using ECommerce.WebApi.Jwt;

namespace ECommerce.WebApi.Controllers
{
    /// Sipariş işlemleri. Giriş zorunlu.
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

      

        // Sipariş oluştur
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderRequest request)
        {
            //hangi kullanıcının sipariş verdiğini bilmek için token dan user ıd'yi çekiyorum 
            var idClaim = User.FindFirst(JwtClaimNames.Id)?.Value;

            //userId'yi kontrol ediyorum geçersizse Unauthorized ile mesaj dönüyorum.
            if (!int.TryParse(idClaim, out var userId) || userId <= 0)
            {
                return Unauthorized("Geçersiz kullanıcı bilgisi.");
            }

            //servis katmanına göndermek üzere request üzerindeki bilgilerden dto oluşturuyorum.
            var addOrderDto = new AddOrderDto
            {
                UserId = userId,
                Items = request.Products.Select(x => new OrderProductItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                }).ToList()
            };

            //_orderService üzerinden AddOrderı çağırıp dönen servis mesajı result değişkenine atıyorum
            var result = await _orderService.AddOrder(addOrderDto, userId);

            //result'ın IsSucceed'i false ise BadRequest ile result içindeki mesajı döndürüyorum
            if (!result.IsSucceed)
                return BadRequest(result.Message);
            //true ise Ok ile result içindeki mesajı döndürüyorum
            else
                return Ok(result.Message);
        }

        // Kullanıcının kendi siparişleri
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var (userId, _) = GetUser();
            return Ok(await _orderService.GetOrders(userId));
        }

        // Tek sipariş getir (admin herkesinkini görebilir; user sadece kendininkini)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var (userId, isAdmin) = GetUser();
            var o = await _orderService.GetOrder(id, userId, isAdmin);
            if (o == null)
                return NotFound();
            return Ok(o);
        }

        // Sipariş sil (admin veya sipariş sahibi)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
          
            var (userId, isAdmin) = GetUser();
            var result = await _orderService.DeleteOrder(id, userId, isAdmin);
            if (!result.IsSucceed)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        // Token'dan kullanıcı bilgisini çekmek için yardımcı
        private (int userId, bool isAdmin) GetUser()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";
            return (int.Parse(idStr), role.Equals("Admin", StringComparison.OrdinalIgnoreCase));
        }
    }

}

