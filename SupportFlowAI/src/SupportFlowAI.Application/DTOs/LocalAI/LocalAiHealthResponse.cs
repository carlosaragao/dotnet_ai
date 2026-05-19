namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalAiHealthResponse(
    bool IsAvailable,
    string BaseUrl,
    string? Error
);