namespace Agents_v._1.Interface
{
    public interface IAiModelClient
    {

        Task<string> CreateChatCompletionAsync(string json);
    }
}
