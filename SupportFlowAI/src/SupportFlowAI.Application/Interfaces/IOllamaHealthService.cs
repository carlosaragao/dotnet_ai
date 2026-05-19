using SupportFlowAI.Application.DTOs.LocalAI;

namespace SupportFlowAI.Application.Interfaces;

public interface IOllamaHealthService
{
    Task<LocalAiHealthResponse> CheckAsync(CancellationToken cancellationToken);
}