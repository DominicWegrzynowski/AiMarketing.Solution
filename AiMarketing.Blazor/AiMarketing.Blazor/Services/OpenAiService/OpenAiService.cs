using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.DTOs;
using AiMarketing.Blazor.Models.Enums;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace AiMarketing.Blazor.Services.OpenAiService
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public OpenAiService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }
        public async Task<List<string>> GenerateMarketingPostsAsync(BusinessProfileDTO profile, int numberOfPosts)
        {
            // Dynamically set how long the post can be based on numberOfPosts
            var maxTokens = 3000;

            string brandVoiceString = profile.BrandVoices != null && profile.BrandVoices.Any()
                                    ? string.Join(", ", profile.BrandVoices)
                                    : "a neutral and engaging tone";

            var prompt = $"Generate {numberOfPosts} social media posts for a {profile.Industry} business named {profile.BusinessName} that offers {profile.Services} in a {brandVoiceString} tone. Make it engaging and creative keeping up to date with the industry standards of the {profile.Industry} business. Don't prefix each one with numbers or bullets, Just list them.";

            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a marketing AI that creates engaging social media posts."},
                    new { role = "user", content=prompt}
                },
                max_tokens = maxTokens
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("chat/completions", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API all failed: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            var posts = new List<string>();
            var postsContent = result.GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();


            //Split posts based on a custom delimiter
            var splitPosts = postsContent?.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitPosts is not null)
            {
                posts.AddRange(splitPosts);
            }

            return posts;

        }
    }
}
