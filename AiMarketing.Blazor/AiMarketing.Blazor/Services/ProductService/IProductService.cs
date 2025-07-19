using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.Enums;

namespace AiMarketing.Blazor.Services.ProductService
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByRoleAsync(UserRole role);
    }
}
