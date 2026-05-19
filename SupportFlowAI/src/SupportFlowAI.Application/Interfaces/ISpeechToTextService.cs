using SupportFlowAI.Application.AI.Media;

namespace SupportFlowAI.Application.Interfaces;

public interface ISpeechToTextService
{
    Task<SpeechToTextResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        CancellationToken cancellationToken);
}