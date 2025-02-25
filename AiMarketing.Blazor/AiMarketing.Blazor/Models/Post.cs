using AiMarketing.Blazor.Data;
using System.ComponentModel.DataAnnotations;

namespace AiMarketing.Blazor.Models
{
    public class Post
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsPosted { get; set; }

        public ApplicationUser User { get; set; }
    }
}
