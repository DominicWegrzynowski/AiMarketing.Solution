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
            var profile = await _context.BusinessProfiles
                                        .Include(bp => bp.BrandVoices)
                                        .FirstOrDefaultAsync(bp => bp.UserId == userId);
                            

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
                                        .Include(bp => bp.BrandVoices)
                                        .FirstOrDefaultAsync(bp => bp.UserId == profile.UserId);

                if (existingProfile is null) 
                {
                    profile.UserId = profile.UserId ?? throw new ArgumentException("UserId is required.");

                    await _context.BusinessProfiles.AddAsync(profile);
                }
                else
                {
                    //Update the existing profile with new values 
                    existingProfile.BusinessName = profile.BusinessName;
                    existingProfile.Industry = profile.Industry;
                    existingProfile.Services = profile.Services;
                    existingProfile.TargetAudience = profile.TargetAudience;

                    //Clear out old BrandVoices if necessary
                    var brandVoicesToRemove = existingProfile.BrandVoices 
                                                             .Where(bv => !profile.BrandVoices
                                                                .Any(newBv => newBv.Voice == bv.Voice))
                                                             .ToList();

                    // Remove BrandVoices that are no longer selected
                    _context.BrandVoices.RemoveRange(brandVoicesToRemove);

                    // Add the new BrandVoices (if any)
                    foreach(var brandVoice in profile.BrandVoices)
                    {
                        if(!existingProfile.BrandVoices.Any(bv => bv.Voice == brandVoice.Voice))
                        {
                            existingProfile.BrandVoices.Add(brandVoice);
                        }
                    }
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
