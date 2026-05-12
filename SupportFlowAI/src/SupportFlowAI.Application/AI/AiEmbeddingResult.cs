using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Application.AI;

public sealed record AiEmbeddingResult(
    EmbeddingVector Vector,
    string Model,
    int PromptTokens,
    int TotalTokens
);