 using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApi.Models
{
    //Ürün eklenmek istediğinde girilmesini istediğim değerleri burada tutuyorum
    public class AddProductRequest
	{
        [Required]
        [MaxLength(50)]
        public string ProductName { get; set; }

        //Fiyat bilgisi en az 0.01 en fazla double.Max olmalı 
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat bilgisi 0'dan büyük olmalı.")]
        public decimal Price { get; set; }

        //stok bilgisi de 0'dan büyük olmalı stoğu olmayan ürün eklenememeli
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Stok bilgisi 0'dan büyük olmalı")]
        public int StockQuantity { get; set; }
    }
}

