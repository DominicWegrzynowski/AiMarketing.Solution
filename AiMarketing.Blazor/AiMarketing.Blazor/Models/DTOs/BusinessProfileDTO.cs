namespace AiMarketing.Blazor.Models.DTOs
{
    public class BusinessProfileDTO
    {
        public string BusinessName { get; set; }
        public string Industry { get; set; }
        public string Services { get; set; }
        public string TargetAudience { get; set; }
        public List<string> BrandVoices { get; set; } = new();
    }
}
