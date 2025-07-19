using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.DTOs;
using AiMarketing.Blazor.Services.OpenAiService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AiMarketing.Blazor.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOpenAiService _openAiService;
        public PostService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOpenAiService openAiService)
        {
            _userManager = userManager;
            _context = context;
            _openAiService = openAiService;
        }

        public async Task AddPostsAsync(List<Post>? posts)
        {
            try
            {
                await _context.Posts.AddRangeAsync(posts!);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            { 
                throw;
            }

        }

        public async Task EnsurePostAllowanceAsync(ApplicationUser user, int numberOfPosts)
        {
            await SetMonthlyPostAllowanceAsync(user);

            if (numberOfPosts > user.MonthlyPostAllowance)
                throw new InvalidOperationException("You do not have enough posts available this month");
        }

        public async Task GenerateAndStorePostsAsync(ApplicationUser user, BusinessProfileDTO profile, int numberOfPosts)
        {
            //Ensure the user's post allowance is up to date
            await EnsurePostAllowanceAsync(user, numberOfPosts);

            //Generate posts if the user has sufficient allowance
            var aiPosts = await _openAiService.GenerateMarketingPostsAsync(profile, numberOfPosts);


            var posts = aiPosts.Select(p => new Post
            {
                UserId = user.Id,
                Content = p.Content,
                ImageRecommendation = p.ImageRecommendation,
                ScheduledTime = null,
                IsPosted = false,
            }).ToList();

            await AddPostsAsync(posts);

            user.MonthlyPostAllowance -= numberOfPosts;
            await _userManager.UpdateAsync(user);

        }

        public async Task<Post> GetPostAsync(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post is null)
                throw new KeyNotFoundException("Post not found");

            return post;
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId)
        {
            try
            {
                var posts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
                return posts;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetMonthlyPostAllowanceAsync(ApplicationUser user)
        {
            var currentDate = DateTime.UtcNow;

            //If the user's allowance period has ended reset their allowance
            if(user.AllowanceEnd < currentDate)
            {
                user.MonthlyPostAllowance = 5; //Default for TrialUser
                user.AllowanceStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                user.AllowanceEnd = user.AllowanceStart.AddMonths(1).AddDays(-1);

                if(await _userManager.IsInRoleAsync(user, "BasicUser"))
                {
                    user.MonthlyPostAllowance = 30;
                }
                else if(await _userManager.IsInRoleAsync(user, "PremiumUser"))
                {
                    user.MonthlyPostAllowance = 30;
                }

                await _userManager.UpdateAsync(user);
            }
        }

        public async Task UpdatePostAsync(Post updatedPost)
        {
            var existingPost = await _context.Posts.FindAsync(updatedPost.Id);
            if (existingPost is null)
                throw new KeyNotFoundException("Post not found.");

            existingPost.Content = updatedPost.Content;
            existingPost.ScheduledTime = updatedPost.ScheduledTime;
            existingPost.IsPosted = updatedPost.IsPosted;

            await _context.SaveChangesAsync();
        }
    }
}
