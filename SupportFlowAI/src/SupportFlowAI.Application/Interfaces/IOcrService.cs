using SupportFlowAI.Application.AI.Media;

namespace SupportFlowAI.Application.Interfaces;

public interface IOcrService
{
    Task<OcrResult> ExtractTextAsync(
        Stream imageStream,
        CancellationToken cancellationToken);
}