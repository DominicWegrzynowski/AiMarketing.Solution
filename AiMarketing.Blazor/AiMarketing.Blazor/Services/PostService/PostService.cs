using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

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

        public async Task AddPostsAsync(List<string>? postContents, string userId)
        {
            try
            {
                var posts = postContents!.Select(content => new Post
                {
                    UserId = userId,
                    Content = content,
                    ScheduledTime = null,
                    IsPosted = false,
                }).ToList();

                await _context.Posts.AddRangeAsync(posts);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            { 
                throw;
            }

        }

        public Task<Post> GetPostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Post>> GetUserPosts(string userId)
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
    }
}
