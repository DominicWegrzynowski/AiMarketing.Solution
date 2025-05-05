using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AiMarketing.Blazor.Data
{
    
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string StripeCustomerId { get; set; } = "";
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public int MonthlyPostAllowance { get; set; } = 5; // Default 5 for TrialUser
        public DateTime AllowanceStart { get; set; } = DateTime.UtcNow; //Default current date
        public DateTime AllowanceEnd { get; set; } = DateTime.UtcNow.AddMonths(1).AddDays(-1); // Default to the end of the current month
    }

}
