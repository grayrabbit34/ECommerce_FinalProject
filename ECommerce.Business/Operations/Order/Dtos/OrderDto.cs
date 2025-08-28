using System;
namespace ECommerce.Business.Operations.Order.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderProductItemDto> Products { get; set; } = new();
    }

}

