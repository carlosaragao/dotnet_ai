namespace SupportFlowAI.Application.DTOs.FoundryAI;

public sealed record FoundryAiCompletionRequest(
    string Input,
    string? Instructions,
    double? Temperature,
    int? MaxOutputTokens
);