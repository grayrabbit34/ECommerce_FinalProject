using System;
namespace ECommerce.Business.Operations.Product.Dtos
{
    public class AddProductDto
    {
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}

