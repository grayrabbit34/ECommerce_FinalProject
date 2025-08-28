using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApi.Models
{
	public class AddOrderRequest
	{
        //bir siparişte birden fazla ürün toplu şekilde sipariş edilmiş olabileceği için ürünleri bir liste üzerinden alıyorum
        [Required]
        public List<OrderProductRequest> Products { get; set; }
    }

    public class OrderProductRequest
    {
        //her bir liste itemının ürün id'si ve kaç adet sipariş edilmek istendiğine dair stock quantity'si olacak

        [Required]
        public int ProductId { get; set; }

        [ Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

}

