using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApi.Models
{
	public class UpdateProductRequest
	{
        //Update edilecek veriye ait bilgilerin olduğu model
        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Stok bilgisi 0'dan büyük olmalı")]
        public int StockQuantity { get; set; }
    }
}

