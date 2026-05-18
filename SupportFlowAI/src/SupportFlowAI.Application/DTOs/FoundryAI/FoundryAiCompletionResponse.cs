namespace SupportFlowAI.Application.DTOs.FoundryAI;

public sealed record FoundryAiCompletionResponse(
    string Provider,
    string Model,
    string Response,
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    string? Status
);