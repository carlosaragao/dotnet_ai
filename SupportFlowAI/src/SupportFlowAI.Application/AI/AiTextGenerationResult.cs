namespace SupportFlowAI.Application.AI;

public sealed record AiTextGenerationResult(
    string Content,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    string? FinishReason
);