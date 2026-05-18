using SupportFlowAI.Application.DTOs.FoundryAI;

namespace SupportFlowAI.Application.Interfaces;

public interface IFoundryAiService
{
    Task<FoundryAiCompletionResponse> GenerateAsync(
        FoundryAiCompletionRequest request,
        CancellationToken cancellationToken);
}