using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace AiMarketing.Blazor.Services.AllowanceService
{
    public class AllowanceService : IAllowanceService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AllowanceService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

        public int GetAllowanceByRole(UserRole role) => role switch
        {
            UserRole.PremiumUser => 60,
            UserRole.BasicUser => 30,
            _ => 5
        };

        public async Task<UserRole> GetUserRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? nameof(UserRole.TrialUser);

            return Enum.TryParse<UserRole>(roleName, out var roleEnum) ? roleEnum : UserRole.TrialUser;
        }

        public async Task RefreshAllowanceIfNeededAsync(ApplicationUser user)
        {
            if(DateTime.UtcNow > user.AllowanceEnd)
            {
                var role = await GetUserRoleAsync(user);

                user.MonthlyPostAllowance = GetAllowanceByRole(role);
                user.AllowanceStart = DateTime.UtcNow;
                user.AllowanceEnd = DateTime.UtcNow.AddMonths(1).AddDays(-1);

                await _userManager.UpdateAsync(user);
            }
        }
    }
}
