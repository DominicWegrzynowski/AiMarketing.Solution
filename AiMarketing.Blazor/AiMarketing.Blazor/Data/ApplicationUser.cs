using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AiMarketing.Blazor.Models.Enums;

namespace AiMarketing.Blazor.Data
{
    
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        // Current Cycle Entitlements(what user can use right now)
        public int MonthlyPostAllowance { get; set; } = 3; // Default 3 for TrialUser
        public int PostsUsedThisPeriod { get; set; } = 0;
        public DateTime? CurrentPeriodStartUtc { get; set; }
        public DateTime? CurrentPeriodEndUtc { get; set; }
        public DateTime? SubscriptionStart { get; set; }
        public DateTime? SubscriptionEnd { get; set; }

        // Stripe identifiers / status
        [Required]
        public string StripeCustomerId { get; set; } = "";
        public string? StripeSubscriptionId { get; set; }
        public string? SubscriptionStatus { get; set; } // "active", "past_due", "canceled", etc.

        // ---Tiers---
        // Current enforcemnt
        public UserRole SubscriptionTier { get; set; } = UserRole.Trial; // Default to trail

        // The tier that will take effect next cycle( set on mid cycle plan change)
        public UserRole? EntitlementTierNext { get; set; }
        // Obsolete - to remove later once safe
        [Obsolete("Use CurrentPeriodStartUtc instead")]
        public DateTime AllowanceStart { get; set; } = DateTime.UtcNow; //Default current date
        [Obsolete("Use CurrentPeriodEndUtc instead")]
        public DateTime AllowanceEnd { get; set; } = DateTime.UtcNow.AddMonths(1).AddDays(-1); // Default to the end of the current month

    }

}
