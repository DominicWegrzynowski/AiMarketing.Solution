using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.DTOs;

namespace AiMarketing.Blazor.Services.OpenAiService
{
    public interface IOpenAiService
    {
        Task<List<string>> GenerateMarketingPostsAsync(BusinessProfileDTO profile, int numberOfPosts);
    }
}