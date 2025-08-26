using System;

using ECommerce.Business.Types;
using ECommerce.Data.Entities;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWork;



namespace ECommerce.Business.Operations.Order.Dtos
{
    
    //çoka çok bağlantı olduğu için birden fazla reponun dependency injection'ını yapmamız gerekiyor
    public class OrderManager : IOrderService
    {
        private readonly IRepository<OrderEntity> _orderRepo;
        private readonly IRepository<ProductEntity> _productRepo;
        private readonly IRepository<OrderProductEntity> _orderProductRepo;
        private readonly IUnitOfWork _uow;

        public OrderManager(IRepository<OrderEntity> orderRepo,
                            IRepository<ProductEntity> productRepo,
                            IRepository<OrderProductEntity> orderProductRepo,
                            IUnitOfWork uow)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _orderProductRepo = orderProductRepo;
            _uow = uow;
        }

        public async Task<ServiceMessage<OrderDto>> AddOrder(AddOrderDto dto, int userId)
        {
            if (dto.Items.Count == 0)
                return new ServiceMessage<OrderDto> { IsSucceed = false, Message = "Sipariş boş olamaz." };

            // Stok kontrolü & toplam tutar
            decimal total = 0;
            foreach (var item in dto.Items)
            {
                var product = _productRepo.Get(x => x.Id == item.ProductId);
                if (product == null)
                    return new ServiceMessage<OrderDto> { IsSucceed = false, Message = $"Ürün bulunamadı: {item.ProductId}" };

                if (product.StockQuantity < item.Quantity)
                    return new ServiceMessage<OrderDto> { IsSucceed = false, Message = $"Stok yetersiz: {product.ProductName}" };

                total += product.Price * item.Quantity;
            }

            var order = new OrderEntity
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = total
            };
            _orderRepo.Add(order);
            await _uow.SaveChangesAsync(); // Id üretimi için

            // İlişki ve stok düşümü
            foreach (var item in dto.Items)
            {
                _orderProductRepo.Add(new OrderProductEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });

                var p = _productRepo.Get(x => x.Id == item.ProductId)!;
                p.StockQuantity -= item.Quantity;
                p.ModifiedDate = DateTime.UtcNow;
                _productRepo.Update(p);
            }

            await _uow.SaveChangesAsync();

            return new ServiceMessage<OrderDto>
            {
                IsSucceed = true,
                Data = new OrderDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Products = dto.Items
                }
            };
        }

        public Task<List<OrderDto>> GetOrders(int? userId = null)
        {
            var q = _orderRepo.GetAll();
            if (userId.HasValue) q = q.Where(o => o.UserId == userId.Value);

            var list = q.OrderByDescending(o => o.Id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Products = o.OrderProducts.Select(op => new OrderProductItemDto
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                }).ToList();

            return Task.FromResult(list);
        }

        public Task<OrderDto?> GetOrder(int id, int? requesterUserId = null, bool isAdmin = false)
        {
            var o = _orderRepo.Get(x => x.Id == id);
            if (o == null) return Task.FromResult<OrderDto?>(null);

            if (!isAdmin && requesterUserId.HasValue && o.UserId != requesterUserId.Value)
                return Task.FromResult<OrderDto?>(null);

            var dto = new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Products = o.OrderProducts.Select(op => new OrderProductItemDto
                {
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                }).ToList()
            };

            return Task.FromResult<OrderDto?>(dto);
        }

        public async Task<ServiceMessage> DeleteOrder(int id, int requesterUserId, bool isAdmin)
        {
            var order = _orderRepo.Get(x => x.Id == id);
            if (order == null) return new ServiceMessage { IsSucceed = false, Message = "Sipariş bulunamadı." };

            if (!isAdmin && order.UserId != requesterUserId)
                return new ServiceMessage { IsSucceed = false, Message = "Yetkisiz işlem." };

            // Stok iadesi
            foreach (var op in order.OrderProducts.ToList())
            {
                var p = _productRepo.Get(x => x.Id == op.ProductId);
                if (p != null)
                {
                    p.StockQuantity += op.Quantity;
                    p.ModifiedDate = DateTime.UtcNow;
                    _productRepo.Update(p);
                }
            }

            _orderRepo.Delete(order);
            await _uow.SaveChangesAsync();
            return new ServiceMessage { IsSucceed = true, Message = "Sipariş silindi." };
        }
    }

}

