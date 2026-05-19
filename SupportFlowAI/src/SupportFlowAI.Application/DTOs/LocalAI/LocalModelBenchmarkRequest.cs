namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalModelBenchmarkRequest(
    string Prompt,
    IReadOnlyCollection<string> Models,
    int Runs = 1,
    double Temperature = 0.2,
    int MaxOutputTokens = 200
);