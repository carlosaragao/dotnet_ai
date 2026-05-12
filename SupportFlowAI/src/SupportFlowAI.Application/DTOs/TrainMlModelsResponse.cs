namespace SupportFlowAI.Application.DTOs;

public sealed record TrainMlModelsResponse(
    string Message,
    string CategoryModelPath,
    string EffortModelPath
);