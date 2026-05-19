namespace SupportFlowAI.Application.Interfaces;

public interface IAiModelSettings
{
    string DefaultModel { get; }
    IReadOnlyCollection<string> AvailableModels { get; }
}