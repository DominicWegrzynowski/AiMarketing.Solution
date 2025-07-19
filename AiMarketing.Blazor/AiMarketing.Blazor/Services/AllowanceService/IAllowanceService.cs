using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models.Enums;

namespace AiMarketing.Blazor.Services.AllowanceService
{
    public interface IAllowanceService
    {
        Task<UserRole> GetUserRoleAsync(ApplicationUser user);
        Task RefreshAllowanceIfNeededAsync(ApplicationUser user);
        Task<bool> CanGeneratePostsAsync(ApplicationUser user, int numberOfRequestedPosts);
        Task DeductPostsAsync(ApplicationUser user, int numberOfPosts);
        Task<int> GetAllowanceByRole(UserRole role);
    }
}

