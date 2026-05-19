namespace SupportFlowAI.Application.AI;

public sealed record AiUsageRecord(
    Guid Id,
    string Provider,
    string Operation,
    string Model,
    int EstimatedInputTokens,
    int? ActualInputTokens,
    int? ActualOutputTokens,
    int? ActualTotalTokens,
    int MaxOutputTokens,
    DateTime CreatedAt
);