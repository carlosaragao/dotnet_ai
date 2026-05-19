namespace SupportFlowAI.Application.DTOs.LocalAI;

public sealed record LocalAiChatResponse(
    string Provider,
    string Model,
    string Response,
    long TotalDurationNanoseconds,
    long LoadDurationNanoseconds,
    int PromptEvalCount,
    long PromptEvalDurationNanoseconds,
    int EvalCount,
    long EvalDurationNanoseconds,
    double TokensPerSecond
);