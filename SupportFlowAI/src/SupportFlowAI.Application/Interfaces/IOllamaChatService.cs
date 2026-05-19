using SupportFlowAI.Application.DTOs.LocalAI;

namespace SupportFlowAI.Application.Interfaces;

public interface IOllamaChatService
{
    Task<LocalAiChatResponse> SendAsync(
        LocalAiChatRequest request,
        CancellationToken cancellationToken);
}