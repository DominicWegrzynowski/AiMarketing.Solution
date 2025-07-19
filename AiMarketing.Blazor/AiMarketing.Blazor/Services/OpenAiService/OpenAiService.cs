using AiMarketing.Blazor.Models;
using AiMarketing.Blazor.Models.DTOs;
using AiMarketing.Blazor.Models.Enums;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        public async Task<List<AiGeneratedPostDTO>> GenerateMarketingPostsAsync(BusinessProfileDTO profile, int numberOfPosts)
        {
            var prompt = BuildPrompt(profile, numberOfPosts);
            var raw = await SendOpenAiPromptAsync(prompt);
            var parsed = CleanPostContent(raw);
            var isValid = ValidatePostContent(parsed, numberOfPosts);

            if (!isValid)
            {
                raw = await SendOpenAiPromptAsync(prompt);
                parsed = CleanPostContent(raw);
                isValid = ValidatePostContent(parsed, numberOfPosts);

                if (!isValid)
                    throw new Exception($"GPT validation failed. Expected {numberOfPosts} posts, got {parsed.Count}.");
            }

            return parsed;


        }
        private string BuildPrompt(BusinessProfileDTO profile, int numberOfPosts)
        {
            string brandVoiceString = profile.BrandVoices != null && profile.BrandVoices.Any()
                                    ? string.Join(", ", profile.BrandVoices)
                                    : "a neutral and engaging tone";

            var prompt = $$"""
                            Generate exactly {{numberOfPosts}} short social media posts for a {{profile.Industry}} business named {{profile.BusinessName}} that offers {{profile.Services}}, written in a {{brandVoiceString}} tone.

                            Each post must:
                            - Be engaging and creative
                            - Be no more than 80 words
                            - Include a short description of a relevant stock image

                            Return the result strictly in the following JSON format:
                            [
                              {
                                "Content": "social media post here",
                                "ImageRecommendation": "Short image description here"
                              } 
                            ]

                            Important:
                            - Do not include any explanation, commentary, or extra formatting—**return only the raw JSON array**
                            - Follow the JSON format exactly to ensure parsing
                        """
                        ;

            return prompt;
        }

        private async Task<string> SendOpenAiPromptAsync(string prompt)
        {
            var maxTokens = 3000;

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

            Console.WriteLine("RAW CHATGPT RESPONSE:");
            Console.WriteLine($"{postsContent}");

            return postsContent ?? string.Empty;
        }

        private List<AiGeneratedPostDTO> CleanPostContent(string rawResponse)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<AiGeneratedPostDTO>>(rawResponse, options) ?? new List<AiGeneratedPostDTO>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Failed to parse JSON: " + ex.Message);
                return new List<AiGeneratedPostDTO>();
            }
        }

        private bool ValidatePostContent(List<AiGeneratedPostDTO> content, int expectedCount)
        {
            if (content is null || content.Count != expectedCount)
                return false;

            return content.All(p =>
                !string.IsNullOrWhiteSpace(p.Content) &&
                !string.IsNullOrWhiteSpace(p.ImageRecommendation));
        }
    }
}
