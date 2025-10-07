using AiMarketing.Blazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<BusinessProfile> BusinessProfiles { get; set; }
        public DbSet<BrandVoice> BrandVoices { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BusinessProfile>()
                   .HasOne<ApplicationUser>()
                   .WithMany()  // User has many business profiles (inverse relationship)
                   .HasForeignKey(bp => bp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var timestamp = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<Post>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedWhen = timestamp;
                    entry.Entity.ModifiedWhen = timestamp;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedWhen = timestamp;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
