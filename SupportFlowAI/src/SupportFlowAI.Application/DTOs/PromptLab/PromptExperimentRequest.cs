using SupportFlowAI.Application.Prompts;

namespace SupportFlowAI.Application.DTOs.PromptLab;

public sealed record PromptExperimentRequest(
    string Input,
    string Task,
    PromptStyle Style,
    string? Model,
    double Temperature = 0.2,
    double? TopP = null,
    int MaxOutputTokens = 500
);