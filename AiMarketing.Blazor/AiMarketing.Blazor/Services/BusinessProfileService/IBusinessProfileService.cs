using AiMarketing.Blazor.Models;

namespace AiMarketing.Blazor.Services.BusinessProfileService
{
    public interface IBusinessProfileService
    {
        public Task SaveProfileAsync(BusinessProfile profile);
        public Task<BusinessProfile> GetProfileByUserIdAsync(string userId);
    }
}
