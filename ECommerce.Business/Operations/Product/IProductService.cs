using System;


using ECommerce.Business.Operations.Product.Dtos;
using ECommerce.Business.Types;

namespace ECommerce.Business.Operations.Product
{
   

    public interface IProductService
    {
        Task<ServiceMessage> AddProduct(AddProductDto product);
        Task<ServiceMessage> UpdateProduct(UpdateProductDto product);
        Task<ServiceMessage> DeleteProduct(int id);
        Task<ProductDto?> GetProduct(int id); 
        Task<List<ProductDto>> GetProducts();
        Task<ServiceMessage> AdjustProductPrice(int id, decimal price);
        Task<ServiceMessage> AdjustProductStockQuantity(int id, int stock);
        Task<PagedResult<ProductDto>> GetPagedProductsAsync(int page, int pageSize);
    }

}

