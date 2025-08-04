using System.Security.Claims;

namespace AiMarketing.Blazor.Services.BillingService
{
    public interface IBillingService
    {
        Task<string?> StartCheckoutAsync(string priceId, ClaimsPrincipal principal);
        Task<string?> CreatePortalSessionAsync(ClaimsPrincipal principal);
        Task DowngradeExpiredUsersAsync();
        Task ResetTrialAllowancesAsync();
    }
}
