using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Business.Operations.Product;
using ECommerce.Business.Operations.Product.Dtos;
using ECommerce.WebApi.Filters;
using ECommerce.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ECommerce.WebApi.Controllers
{

    /// <summary>
    /// Ürün CRUD ve yardımcı işlemleri.
    /// Public GET'ler ve Admin korumalı yazma işlemleri.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;

        }

        // Herkese açık liste
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _productService.GetProducts());

        // Herkese açık sayfalı liste
        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _productService.GetPagedProductsAsync(page, pageSize));

        // Herkese açık ürün detayı
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _productService.GetProduct(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        // Admin -> ürün ekleme
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {

            var addProductDto = new AddProductDto
            {
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            var result = await _productService.AddProduct(addProductDto);
                     
            if (!result.IsSucceed)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        // Admin -> ürün güncelleme (saat filtresi ile kısıtlı)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [TimeControlFilter(StartTime = "15:00", EndTime = "23:59")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.Id) return BadRequest("Id uyuşmuyor.");
            var res = await _productService.UpdateProduct(dto);
            if (!res.IsSucceed) return BadRequest(res.Message);
            return Ok(res.Message);
        }

        // Admin -> sadece fiyat güncelle
        [HttpPatch("{id:int}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustPrice(int id, [FromQuery] decimal price)
        {
            var res = await _productService.AdjustProductPrice(id, price);
            if (!res.IsSucceed) return BadRequest(res.Message);
            return Ok(res.Message);
        }

        // Admin -> sadece stok güncelle
        [HttpPatch("{id:int}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustStock(int id, [FromQuery] int stock)
        {
            var res = await _productService.AdjustProductStockQuantity(id, stock);
            if (!res.IsSucceed) return BadRequest(res.Message);
            return Ok(res.Message);
        }

        // Admin -> silme
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result.IsSucceed) return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }

}

