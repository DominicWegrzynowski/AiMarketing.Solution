using AiMarketing.Blazor.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiMarketing.Blazor.Models
{
    public class BusinessProfile
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string BusinessName { get; set; }
        [Required]
        public string Industry { get; set; }
        [Required]
        public string Services { get; set; }
        [Required]
        public string TargetAudiences { get; set; }
        [Required]
        public string BrandVoice { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
