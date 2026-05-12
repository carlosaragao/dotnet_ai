using SupportFlowAI.Application.Prompts;

namespace SupportFlowAI.Application.DTOs.PromptLab;

public sealed record PromptExperimentResponse(
    string Model,
    string Task,
    PromptStyle Style,
    double Temperature,
    double? TopP,
    int MaxOutputTokens,
    string SystemPrompt,
    string FinalPrompt,
    string Output,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    string? FinishReason
);