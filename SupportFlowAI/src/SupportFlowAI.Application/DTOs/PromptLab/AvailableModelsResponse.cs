namespace SupportFlowAI.Application.DTOs.PromptLab;

public sealed record AvailableModelsResponse(
    string DefaultModel,
    IReadOnlyCollection<string> AvailableModels
);