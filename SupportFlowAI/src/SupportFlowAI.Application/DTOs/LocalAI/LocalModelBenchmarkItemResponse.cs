namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalModelBenchmarkItemResponse(
    string Model,
    int Run,
    long ElapsedMilliseconds,
    int PromptEvalCount,
    int EvalCount,
    double TokensPerSecond,
    string ResponsePreview
);