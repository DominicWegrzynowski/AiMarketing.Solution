using AiMarketing.Blazor.Data;
using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.DTOs;

namespace AiMarketing.Blazor.Services.PostService
{
    public interface IPostService
    {
        public Task AddPostsAsync(List<Post>? postContents);
        public Task<Post> GetPostAsync(int postId);
        public Task<List<Post>> GetUserPostsAsync(string userId);
        public Task GenerateAndStorePostsAsync(ApplicationUser user, BusinessProfileDTO profile, int numberOfPosts);
        public Task EnsurePostAllowanceAsync(ApplicationUser user, int numberOfPosts);
        public Task SetMonthlyPostAllowanceAsync(ApplicationUser user);
        public Task UpdatePostAsync(Post updatedPost);
    }
}
