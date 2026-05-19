using SupportFlowAI.Application.AI;

namespace SupportFlowAI.Application.Interfaces;

public interface IAiTextGenerator
{
    Task<AiTextGenerationResult> GenerateAsync(
        AiTextGenerationRequest request,
        CancellationToken cancellationToken);
}