using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByRoleAsync(UserRole role)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Role == role);
        }
    }

}
