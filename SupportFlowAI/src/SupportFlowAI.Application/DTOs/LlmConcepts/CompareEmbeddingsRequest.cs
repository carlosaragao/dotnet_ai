namespace SupportFlowAI.Application.DTOs.LlmConcepts;

public sealed record CompareEmbeddingsRequest(
    string FirstText,
    string SecondText
);