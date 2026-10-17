namespace Agents_v._1.Model
{
    public class PromptModel
    {

        public class GeneratePromptRequest
        {
            public IFormFile? Resume { get; set; }

            public List<string> Skills { get; set; } = new();

            public decimal ExpectedSalaryLpa { get; set; }

            public string WorkPreference { get; set; } = "Remote";

            public int ExperienceYears { get; set; }
        }
    }
}
