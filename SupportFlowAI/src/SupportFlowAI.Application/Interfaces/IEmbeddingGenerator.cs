using SupportFlowAI.Application.AI;

namespace SupportFlowAI.Application.Interfaces;

public interface IEmbeddingGenerator
{
    Task<AiEmbeddingResult> GenerateAsync(
        string text,
        CancellationToken cancellationToken);
}