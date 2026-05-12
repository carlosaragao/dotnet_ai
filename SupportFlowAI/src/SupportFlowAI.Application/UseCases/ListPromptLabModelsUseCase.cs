using SupportFlowAI.Application.DTOs.PromptLab;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class ListPromptLabModelsUseCase
{
    private readonly IAiModelSettings _modelSettings;

    public ListPromptLabModelsUseCase(IAiModelSettings modelSettings)
    {
        _modelSettings = modelSettings;
    }

    public AvailableModelsResponse Handle()
    {
        return new AvailableModelsResponse(
            _modelSettings.DefaultModel,
            _modelSettings.AvailableModels
        );
    }
}