namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalAiChatRequest(
    string Prompt,
    string? SystemPrompt,
    string? Model,
    double? Temperature,
    int? MaxOutputTokens
);