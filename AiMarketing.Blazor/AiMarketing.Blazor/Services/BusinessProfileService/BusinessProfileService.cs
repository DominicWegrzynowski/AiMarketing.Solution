using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Services.BusinessProfileService
{
    public class BusinessProfileService : IBusinessProfileService
    {
        private readonly ApplicationDbContext _context;

        public BusinessProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessProfile> GetProfileByUserIdAsync(string userId)
        {
            var profile = await _context.BusinessProfiles.FirstOrDefaultAsync(bp => bp.UserId == userId);

            if(profile is null)
            {
                return new BusinessProfile();
            }
            else
            {
                return profile;
            }
        }

        public async Task SaveProfileAsync(BusinessProfile profile)
        {
            try
            {
                var existingProfile = await _context.BusinessProfiles
                                                        .FirstOrDefaultAsync(bp => bp.UserId == profile.UserId);

                if (existingProfile is null)
                {
                    _context.BusinessProfiles.Add(profile);
                }
                else
                {
                    existingProfile.BusinessName = profile.BusinessName;
                    existingProfile.Industry = profile.Industry;
                    existingProfile.Services = profile.Services;
                    existingProfile.TargetAudience = profile.TargetAudience;
                    existingProfile.BrandVoices = profile.BrandVoices;

                    // Sync brand voices as string values
                    _context.BrandVoices.RemoveRange(existingProfile.BrandVoices);
                    existingProfile.BrandVoices = profile.BrandVoices;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
