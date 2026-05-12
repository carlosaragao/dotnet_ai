namespace SupportFlowAI.Application.DTOs.LlmConcepts;

public sealed record ExplainLlmConceptResponse(
    string Concept,
    string Explanation,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
);