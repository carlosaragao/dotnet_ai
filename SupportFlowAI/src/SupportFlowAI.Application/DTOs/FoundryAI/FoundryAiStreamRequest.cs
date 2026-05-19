namespace SupportFlowAI.Application.DTOs.FoundryAI;

public sealed record FoundryAiStreamRequest(
    string Input,
    string? Instructions,
    double? Temperature,
    int? MaxOutputTokens
);