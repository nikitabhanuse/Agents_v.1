using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Agents_v._1.Service
{
    public class OpenAiService
    {
        private readonly IHttpClientFactory _httpFactory;

        public OpenAiService(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public async Task<string> CreateChatCompletionAsync(object payload, CancellationToken cancellationToken = default)
        {
            var client = _httpFactory.CreateClient("openai"); // register this named client
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var req = new HttpRequestMessage(HttpMethod.Post, "/v1/chat/completions") { Content = content };
            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            return body;
        }
    }
}
