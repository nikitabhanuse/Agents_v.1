using DocumentFormat.OpenXml.Packaging; // required for WordprocessingDocument
using UglyToad.PdfPig;
using   Agents_v._1.Interface;        
namespace Agents_v._1.Service
{
    public class ResumeTextExtractor : IResumeTextExtractor
    {



        public async Task<string> ExtractTextAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)       
                throw new ArgumentException("Resume file is empty.");

            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException(
                    "Resume must be smaller than 10 MB.");

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            await using var stream = new MemoryStream();

            await file.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            string text;

            switch (extension)
            {
                case ".pdf":
                    // Basic validation: check PDF header magic bytes before parsing
                    var header = new byte[4];
                    stream.Position = 0;
                    if (stream.Read(header, 0, 4) == 4)
                    {
                        var sig = System.Text.Encoding.ASCII.GetString(header);
                        if (!sig.StartsWith("%PDF"))
                        {
                            throw new ArgumentException("Uploaded file does not appear to be a valid PDF.");
                        }
                    }
                    stream.Position = 0;
                    try
                    {
                        using (var pdf = PdfDocument.Open(stream))
                        {
                            text = string.Join(
                                "\n",
                                pdf.GetPages().Select(page => page.Text));
                        }
                    }
                    catch (UglyToad.PdfPig.Core.PdfDocumentFormatException ex)
                    {
                        throw new ArgumentException($"Unable to parse PDF: {ex.Message}", ex);
                    }
                    break;

                case ".docx":
                    using (var document =
                        WordprocessingDocument.Open(stream, false))
                    {
                        text = document.MainDocumentPart?
                            .Document.Body?.InnerText ?? string.Empty;
                    }
                    break;

                default:
                    throw new ArgumentException(
                        "Only PDF and DOCX files are supported.");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    "No readable text found. The resume may be scanned.");
            }

            return text;
        }
    }
}