using AiMarketing.Blazor.Models;

namespace AiMarketing.Blazor.Services.PostService
{
    public interface IPostService
    {
        public Task<Post> GetPostAsync(int postId);
        public Task<Post> GetUserPosts(string userId);
        public Task<Post> GeneratePostAsync(string userId, string businessType);
    }
}
