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
        [ForeignKey("UserId")]
        public string UserId { get; set; } = "";
        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; } = "";
        [Required]
        public string Industry { get; set; } = "";
        [Required]
        public string Services { get; set; } = "";
        [Required]
        public string TargetAudience { get; set; } = "";
        [Required]
        //One-to-many relationship
        public List<BrandVoice> BrandVoices { get; set; } = new List<BrandVoice>();
    }
}
