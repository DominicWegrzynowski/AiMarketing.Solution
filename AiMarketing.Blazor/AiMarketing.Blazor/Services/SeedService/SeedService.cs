using AiMarketing.Blazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using AiMarketing.Blazor.Models.Enums;

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
    }
}
