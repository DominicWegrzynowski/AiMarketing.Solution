using AiMarketing.Blazor.Models;

namespace AiMarketing.Blazor.Services.PostService
{
    public interface IPostService
    {
        public Task AddPostsAsync(List<string>? postContents, string userId);
        public Task<Post> GetPostAsync(int postId);
        public Task<List<Post>> GetUserPosts(string userId);
    }
}
