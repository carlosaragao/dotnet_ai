namespace SupportFlowAI.Application.AI;

public sealed record AiTextGenerationRequest(
    string Prompt,
    string? SystemPrompt,
    double Temperature,
    int MaxOutputTokens
);