using System;

using ECommerce.Business.Operations.Product.Dtos;
using ECommerce.Business.Types;
using ECommerce.Data.Entities;
using ECommerce.Data.Repositories;
using ECommerce.Data.UnitOfWork;

using Microsoft.EntityFrameworkCore;


namespace ECommerce.Business.Operations.Product
{
    

    public class ProductManager : IProductService
    {

        private readonly IUnitOfWork _uow;

        private readonly IRepository<ProductEntity> _repository; 

        public ProductManager(IRepository<ProductEntity> productRepo, IUnitOfWork uow)
        {
            _repository = productRepo;
            _uow = uow;
        }

        public async Task<ServiceMessage> AddProduct(AddProductDto dto)
        {
            var exists = _repository.GetAll(x => x.ProductName.ToLower() == dto.ProductName.ToLower()).Any();
            if (exists)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün adı zaten mevcut."
                };
            }
            _repository.Add(new ProductEntity
            {
                ProductName = dto.ProductName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            });

            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Ürün eklenirken bir hata oluştu");
            }
            
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Ürün eklendi."
            };
        }

        public async Task<ServiceMessage> UpdateProduct(UpdateProductDto dto)
        {
            var entity = _repository.Get(x => x.Id == dto.Id);
            if (entity == null)
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün bulunamadı."
                };

            entity.ProductName = dto.ProductName;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;
            entity.ModifiedDate = DateTime.UtcNow;

            _repository.Update(entity);
            await _uow.SaveChangesAsync();
            return new ServiceMessage {
                IsSucceed = true,
                Message = "Ürün güncellendi."
            };
        }

        public async Task<ServiceMessage> DeleteProduct(int id)
        {
            var entity = _repository.Get(x => x.Id == id);
            if (entity == null) return new ServiceMessage { IsSucceed = false, Message = "Ürün bulunamadı." };
            _repository.Delete(entity);
            await _uow.SaveChangesAsync();
            return new ServiceMessage { IsSucceed = true, Message = "Ürün silindi." };
        }

        public Task<ProductDto?> GetProduct(int id)
        {
            var p = _repository.Get(x => x.Id == id);
            if (p == null) return Task.FromResult<ProductDto?>(null);

            return Task.FromResult<ProductDto?>(new ProductDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            });
        }

        public Task<List<ProductDto>> GetProducts()
        {
            var list = _repository.GetAll()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                }).ToList();
            return Task.FromResult(list);
        }

        public async Task<ServiceMessage> AdjustProductPrice(int id, decimal price)
        {
            var entity = _repository.Get(x => x.Id == id);
            if (entity == null) return new ServiceMessage { IsSucceed = false, Message = "Ürün bulunamadı." };
            entity.Price = price;
            entity.ModifiedDate = DateTime.UtcNow;
            _repository.Update(entity);
            await _uow.SaveChangesAsync();
            return new ServiceMessage { IsSucceed = true, Message = "Fiyat güncellendi." };
        }

        public async Task<ServiceMessage> AdjustProductStockQuantity(int id, int stock)
        {
            var entity = _repository.Get(x => x.Id == id);
            if (entity == null) return new ServiceMessage { IsSucceed = false, Message = "Ürün bulunamadı." };
            entity.StockQuantity = stock;
            entity.ModifiedDate = DateTime.UtcNow;
            _repository.Update(entity);
            await _uow.SaveChangesAsync();
            return new ServiceMessage { IsSucceed = true, Message = "Stok güncellendi." };
        }

        public Task<PagedResult<ProductDto>> GetPagedProductsAsync(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _repository.GetAll();
            var total = query.Count();

            var items = query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToList();

            var result = new PagedResult<ProductDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

            return Task.FromResult(result);
        }
    }

}

