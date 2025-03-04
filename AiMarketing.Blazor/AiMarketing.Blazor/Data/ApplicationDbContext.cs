using AiMarketing.Blazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;
using System.Text.Json;

namespace AiMarketing.Blazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<BrandVoice> BrandVoices { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BusinessProfile>()
                   .HasOne(bp => bp.User)
                   .WithOne()
                   .HasForeignKey<BusinessProfile>(bp => bp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
