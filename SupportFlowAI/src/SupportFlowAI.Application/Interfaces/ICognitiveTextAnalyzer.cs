using SupportFlowAI.Application.AI;

namespace SupportFlowAI.Application.Interfaces;

public interface ICognitiveTextAnalyzer
{
    Task<CognitiveTextAnalysisResult> AnalyzeAsync(
        string text,
        CancellationToken cancellationToken);
}