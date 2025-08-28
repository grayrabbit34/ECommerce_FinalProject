using System;


using ECommerce.Business.Operations.Order.Dtos;
using ECommerce.Business.Types;

namespace ECommerce.Business.Operations.Order
{
   

    public interface IOrderService
    {
        Task<ServiceMessage<OrderDto>> AddOrder(AddOrderDto dto, int userId);
        Task<List<OrderDto>> GetOrders(int? userId = null);
        Task<OrderDto?> GetOrder(int id, int? requesterUserId = null, bool isAdmin = false);
        Task<ServiceMessage> DeleteOrder(int id, int requesterUserId, bool isAdmin);
    }

}

