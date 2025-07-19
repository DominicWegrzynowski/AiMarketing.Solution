using AiMarketing.Blazor.Data;
using System.ComponentModel.DataAnnotations;

namespace AiMarketing.Blazor.Models
{
    public class Post
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageRecommendation { get; set; } = string.Empty;
        public DateTime? ScheduledTime { get; set; }
        public bool IsPosted { get; set; }

        public ApplicationUser User { get; set; }
    }
}
