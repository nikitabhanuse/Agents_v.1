using static Agents_v._1.Model.PromptModel;

namespace Agents_v._1.Interface
{
    public interface IPromptBuilder
    {
       Task<string> Build(
      GeneratePromptRequest request,
      string resumeText);
    }
}
