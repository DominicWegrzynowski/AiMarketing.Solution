using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models.Enums;
using AiMarketing.Blazor.Services.ProductService;
using Microsoft.AspNetCore.Identity;

namespace AiMarketing.Blazor.Services.AllowanceService
{
    public class AllowanceService : IAllowanceService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductService _productService;

        public AllowanceService(UserManager<ApplicationUser> userManager, IProductService productService)
        {
            _userManager = userManager;
            _productService = productService;
        }

        public async Task<bool> CanGeneratePostsAsync(ApplicationUser user, int numberOfRequestedPosts)
        {
            await RefreshAllowanceIfNeededAsync(user);

            return user.MonthlyPostAllowance >= numberOfRequestedPosts;
        }

        public async Task DeductPostsAsync(ApplicationUser user, int numberOfPosts)
        {
            user.MonthlyPostAllowance = Math.Max(0, user.MonthlyPostAllowance - numberOfPosts);
            await _userManager.UpdateAsync(user);
        }

        public async Task<int> GetAllowanceByRole(UserRole role)
        {
            var productByRole = await _productService.GetProductByRoleAsync(role);
            return productByRole?.PostAllowance ?? 0;
        }

        public async Task<UserRole> GetUserRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? nameof(UserRole.Trial);

            return Enum.TryParse<UserRole>(roleName, out var roleEnum) ? roleEnum : UserRole.Trial;
        }

        public async Task RefreshAllowanceIfNeededAsync(ApplicationUser user)
        {
            if(DateTime.UtcNow > user.AllowanceEnd)
            {
                var role = await GetUserRoleAsync(user);

                user.MonthlyPostAllowance = await GetAllowanceByRole(role);
                user.AllowanceStart = DateTime.UtcNow;
                user.AllowanceEnd = DateTime.UtcNow.AddMonths(1).AddDays(-1);

                await _userManager.UpdateAsync(user);
            }
        }
    }
}
