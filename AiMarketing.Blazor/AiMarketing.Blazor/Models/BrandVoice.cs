using AiMarketing.Blazor.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AiMarketing.Blazor.Models
{
    public class BrandVoice
    {
        [Key]
        public int Id { get; set; }
        public string Voice { get; set; }
        public int BusinessProfileId { get; set; }
        public BusinessProfile BusinessProfile { get; set; }
    }
}
