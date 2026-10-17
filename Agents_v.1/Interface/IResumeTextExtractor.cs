namespace Agents_v._1.Interface
{
    public interface IResumeTextExtractor
    {
        Task<string> ExtractTextAsync(
            IFormFile file,
            CancellationToken cancellationToken = default);
    }
}
