namespace SupportFlowAI.Application.DTOs.LlmConcepts;

public sealed record ExplainLlmConceptRequest(
    string Concept,
    string? ExampleText
);