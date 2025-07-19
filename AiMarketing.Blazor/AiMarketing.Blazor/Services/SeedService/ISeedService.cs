using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models.Enums;

namespace AiMarketing.Blazor.Services.SeedService
{
    public interface ISeedService
    {
        Task AssignRoleToUser(ApplicationUser user, UserRole roleName);
        Task SeedRolesAsync();
        Task SeedProductsAsync(ApplicationDbContext context);
    }
}