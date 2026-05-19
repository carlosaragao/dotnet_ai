namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalModelBenchmarkResponse(
    string Prompt,
    IReadOnlyCollection<LocalModelBenchmarkItemResponse> Results
);