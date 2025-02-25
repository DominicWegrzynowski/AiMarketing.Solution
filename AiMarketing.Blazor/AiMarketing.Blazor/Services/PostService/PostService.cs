using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AiMarketing.Blazor.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Post> GeneratePostAsync(string userId, string businessType)
        {
            var postText = GetTemplateForBusinessType(businessType);
            var post = new Post
            {
                UserId = userId,
                Content = postText,
                ScheduledTime = DateTime.UtcNow.AddDays(1),
                IsPosted = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;

        }

        public Task<Post> GetPostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetUserPosts(string userId)
        {
            throw new NotImplementedException();
        }
        //Later swap out for AI-Generated content using OpenAI API
        private string GetTemplateForBusinessType(string businessType) 
        {
            return businessType switch
            {
            "Coffee Shop" => "☕️ Good morning!Start your day with our fresh brew. Visit us today! #CoffeeTime",
            "Fitness Coach" => "💪 Stay strong! Here's your daily motivation: Keep pushing forward! #FitnessGoals",
            _ => "📢 Stay connected with our latest updates. Follow us for more! #BusinessGrowth"
            };
        }
    }
}
