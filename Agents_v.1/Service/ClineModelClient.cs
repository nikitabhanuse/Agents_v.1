using Agents_v._1.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Agents_v._1.Service
{
    public class ClineModelClient : IAiModelClient
    {


        private readonly IHttpClientFactory _httpFactory;

        public ClineModelClient(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // Interface implementation required
        public async Task<string> CreateChatCompletionAsync(string json)
        {
            var client = _httpFactory.CreateClient("openai");
            const int maxAttempts = 3;
            var rand = new Random();

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var resp = await client.PostAsync("/v1/chat/completions", content);
                if (resp.IsSuccessStatusCode)
                    return await resp.Content.ReadAsStringAsync();

                var body = await resp.Content.ReadAsStringAsync();

                if ((int)resp.StatusCode == 429 || resp.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    if (resp.Headers.TryGetValues("Retry-After", out var values) &&
                        int.TryParse(values.FirstOrDefault(), out var retrySeconds) && retrySeconds > 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(retrySeconds));
                    }
                    else
                    {
                        var delayMs = (int)(Math.Pow(2, attempt) * 1000) + rand.Next(0, 250);
                        await Task.Delay(delayMs);
                    }
                    continue; // retry
                }

                // non-retryable error: include body for diagnostics
                throw new HttpRequestException($"OpenAI request failed {(int)resp.StatusCode} {resp.StatusCode}: {body}");
            }

            throw new HttpRequestException("OpenAI request failed after retries due to rate limiting.");
        }

        // Keep existing method and forward to the interface method for compatibility
        public Task<string> TasksAsync(string json) => CreateChatCompletionAsync(json);
    }
}
