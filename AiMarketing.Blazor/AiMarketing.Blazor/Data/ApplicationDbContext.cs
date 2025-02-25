using AiMarketing.Blazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }

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
