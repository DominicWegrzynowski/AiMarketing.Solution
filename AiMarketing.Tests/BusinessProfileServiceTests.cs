using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Services.BusinessProfileService;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AiMarketing.Tests
{
    public class BusinessProfileServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetProfileByUserIdAsync_WhenProfileExists_ReturnsProfile()
        {
            // Arrange 
            var userId = "user123";
            var dbContext = CreateInMemoryDbContext();

            var testProfile = new BusinessProfile
            {
                UserId = userId,
                BusinessName = "Test Co",
                Industry = "Software",
                Services = "Development",
                TargetAudience = "Small Businesses",
                BrandVoices = new List<BrandVoice> { new BrandVoice { Voice = "Professional"} }
            };

            dbContext.BusinessProfiles.Add(testProfile);
            await dbContext.SaveChangesAsync();

            var service = new BusinessProfileService(dbContext);

            // Act
            var result = await service.GetProfileByUserIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.BusinessName.Should().Be("Test Co");
            result.BrandVoices.Should().ContainSingle(bv => bv.Voice == "Professional");
        }

        [Fact]
        public async Task GetProfileByUserIdAsync_WhenProfileDoesNotExist_ReturnsNewEmptyProfile()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var service = new BusinessProfileService(dbContext);

            // Act
            var result = await service.GetProfileByUserIdAsync("nonexisting-user");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(0);
            result.UserId.Should().BeNullOrEmpty();
            result.BusinessName.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task SaveProfileAsync_WhenProfileDoesNotExist_CreatesNewProfile()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var service = new BusinessProfileService(context);

            var profile = new BusinessProfile
            {
                UserId = "user123",
                BusinessName = "NewCo",
                Industry = "Tech",
                Services = "Ai Consulting",
                TargetAudience = "Startups",
                BrandVoices = new List<BrandVoice> { new BrandVoice { Voice = "Startups" } }
            };

            // Act 
            await service.SaveProfileAsync(profile);

            // Assert
            var saved = await context.BusinessProfiles
                                     .Include(b => b.BrandVoices)
                                     .FirstOrDefaultAsync(b => b.UserId == "user123");

            saved.Should().NotBeNull();
            saved.BusinessName.Should().Be("NewCo");
            saved.BrandVoices.Should().ContainSingle(bv => bv.Voice == "Friendly"); 
        }
    }
}
