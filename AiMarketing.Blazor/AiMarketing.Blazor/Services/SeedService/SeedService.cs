using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AiMarketing.Blazor.Services.SeedService
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SeedService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedRolesAsync()
        {
            foreach (var role in Enum.GetValues(typeof(UserRole)))
            {
                var roleExist = await _roleManager.RoleExistsAsync(role.ToString()!);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()!));
                }
            }
        }
        public async Task AssignRoleToUser(ApplicationUser user, UserRole roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName.ToString());
            if (roleExist)
            {
                await _userManager.AddToRoleAsync(user, roleName.ToString());
            }
        }

        public async Task SeedProductsAsync(ApplicationDbContext context)
        {
            var productsToSeed = new List<Product>
            {
                new Product { Role = UserRole.Trial, DisplayName = "Trial", Price = 0.00m, PostAllowance = 3 },
                new Product { Role = UserRole.Starter, DisplayName = "Starter", Price = 19.99m, PostAllowance = 10 },
                new Product { Role = UserRole.Growth, DisplayName = "Growth", Price = 39.99m, PostAllowance = 25 },
                new Product { Role = UserRole. Pro, DisplayName = "Pro", Price = 59.99m, PostAllowance = 35 }   
            };

            foreach (var product in productsToSeed)
            {
                if (!context.Products.Any(p => p.Role == product.Role))
                {
                    context.Products.Add(product);
                }
            }

            await context.SaveChangesAsync();
        }

    }
}
