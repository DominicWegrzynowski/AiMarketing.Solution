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

        // Helper: remaining = allowance - used
        private static int Remaining(ApplicationUser u)
            => Math.Max(0, (u.MonthlyPostAllowance) - (u.PostsUsedThisPeriod));

        public async Task<bool> CanGeneratePostsAsync(ApplicationUser user, int numberOfRequestedPosts)
        {
            // No automatic resets here; webhook drives period changes.
            return Remaining(user) >= numberOfRequestedPosts;
        }

        public async Task DeductPostsAsync(ApplicationUser user, int numberOfPosts)
        {
            // Increment "used" rather than reducing allowance
            var remaining = Remaining(user);
            if (numberOfPosts <= 0 || remaining <= 0) return;

            user.PostsUsedThisPeriod = Math.Min(
                user.PostsUsedThisPeriod + numberOfPosts,
                user.MonthlyPostAllowance
            );

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

        // IMPORTANT: No silent monthly rollover here anymore (webhook handles it).
        // Leave as a no-op so existing calls compile harmlessly.
        public Task RefreshAllowanceIfNeededAsync(ApplicationUser user)
        {
            return Task.CompletedTask;
        }
    }
}
