using Microsoft.Extensions.Options;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed class OpenAiModelSettings : IAiModelSettings
{
    private readonly OpenAiOptions _options;

    public OpenAiModelSettings(IOptions<OpenAiOptions> options)
    {
        _options = options.Value;
    }

    public string DefaultModel => _options.ChatModel;

    public IReadOnlyCollection<string> AvailableModels =>
        _options.AvailableChatModels.Length == 0
            ? [_options.ChatModel]
            : _options.AvailableChatModels;
}