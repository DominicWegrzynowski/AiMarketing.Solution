using AiMarketing.Blazor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiMarketing.Blazor.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public UserRole Role { get; set; } //Enum
        [Required]
        public string DisplayName { get; set; } = ""; // e.g. "Starter", "Pro"
        [Required]
        [Column(TypeName ="decimal(8,2)")]
        public decimal Price { get; set; }
        public int PostAllowance { get; set; }

    }
}
