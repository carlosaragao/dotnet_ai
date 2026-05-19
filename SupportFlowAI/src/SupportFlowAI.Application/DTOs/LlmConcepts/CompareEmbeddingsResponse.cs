namespace SupportFlowAI.Application.DTOs.LlmConcepts;

public sealed record CompareEmbeddingsResponse(
    string FirstText,
    string SecondText,
    string Model,
    int Dimensions,
    double Similarity,
    string Interpretation,
    int FirstTextTokens,
    int SecondTextTokens
);