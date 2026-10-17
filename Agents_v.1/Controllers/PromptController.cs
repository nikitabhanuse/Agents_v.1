using Agents_v._1.Interface;
using Agents_v._1.Model;
using Agents_v._1.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Agents_v._1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly IResumeTextExtractor _resumeTextExtractor;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IAiModelClient _clineModelClient;

        public PromptController(
            IPromptBuilder promptBuilder,
            IResumeTextExtractor resumeExtractor,
            IAiModelClient clineModelClient)
        {
            _promptBuilder = promptBuilder;
            _resumeTextExtractor = resumeExtractor;
            _clineModelClient = clineModelClient;
        }

        [HttpPost("GetPrompt")]
        public async Task<IActionResult> GetPrompt([FromForm] PromptModel.GeneratePromptRequest request)
        {
            // Implement your logic to process the prompt and generate a response
            // For example, you can call an AI model or perform some operations based on the prompt
            // Example response 
            string resumeText = await _resumeTextExtractor.ExtractTextAsync(request.Resume);

            string  prompt = await _promptBuilder.Build(request, resumeText);


            // csharp
            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
        new { role = "system", content = prompt }
        //new { role = "user", content = "Hello" }
    },
                max_tokens = 1300
            };


            string responseFromAiModel = await _clineModelClient.CreateChatCompletionAsync(JsonConvert.SerializeObject(payload));




            var response = new
            {
                Prompt = prompt,
                Response = "prompt created"
            };
            return new JsonResult(response);
        }


    }
}
