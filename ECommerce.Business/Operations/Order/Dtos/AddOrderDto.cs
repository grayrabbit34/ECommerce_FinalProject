using System;
namespace ECommerce.Business.Operations.Order.Dtos
{
    public class AddOrderDto
    {
        public int UserId { get; set; }
        public List<OrderProductItemDto> Items { get; set; } = new();
    }

    public class OrderProductDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}

