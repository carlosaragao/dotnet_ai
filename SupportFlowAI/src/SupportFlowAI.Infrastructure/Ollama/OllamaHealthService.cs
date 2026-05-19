using Microsoft.Extensions.Options;
using SupportFlowAI.Application.DTOs.LocalAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.Ollama;

public sealed class OllamaHealthService : IOllamaHealthService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaHealthService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<LocalAiHealthResponse> CheckAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("tags", cancellationToken);

            return new LocalAiHealthResponse(
                response.IsSuccessStatusCode,
                _options.BaseUrl,
                response.IsSuccessStatusCode ? null : $"Status {(int)response.StatusCode}"
            );
        }
        catch (Exception exception)
        {
            return new LocalAiHealthResponse(
                false,
                _options.BaseUrl,
                exception.Message
            );
        }
    }
}