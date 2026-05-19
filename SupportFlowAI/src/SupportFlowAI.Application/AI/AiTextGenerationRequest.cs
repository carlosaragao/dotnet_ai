namespace SupportFlowAI.Application.AI;

public sealed record AiTextGenerationRequest(
    string Prompt,
    string? SystemPrompt,
    double Temperature,
    int MaxOutputTokens,
    string? Model = null,
    double? TopP = null
);