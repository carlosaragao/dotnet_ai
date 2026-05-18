using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.DTOs.LocalAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.Ollama;

public sealed class OllamaChatService : IOllamaChatService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaChatService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<LocalAiChatResponse> SendAsync(
        LocalAiChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            throw new ArgumentException("O prompt é obrigatório.");

        var model = string.IsNullOrWhiteSpace(request.Model)
            ? _options.DefaultModel
            : request.Model;

        var messages = new List<OllamaMessage>();

        if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            messages.Add(new OllamaMessage("system", request.SystemPrompt));

        messages.Add(new OllamaMessage("user", request.Prompt));

        var ollamaRequest = new OllamaChatApiRequest(
            Model: model,
            Messages: messages,
            Stream: false,
            Options: new OllamaRequestOptions(
                Temperature: request.Temperature ?? _options.DefaultTemperature,
                NumPredict: request.MaxOutputTokens ?? _options.DefaultMaxOutputTokens
            )
        );

        var response = await _httpClient.PostAsJsonAsync(
            "chat",
            ollamaRequest,
            JsonOptions,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Erro ao chamar Ollama. Status: {(int)response.StatusCode}. Detalhes: {error}"
            );
        }

        var result = await response.Content.ReadFromJsonAsync<OllamaChatApiResponse>(
            JsonOptions,
            cancellationToken
        );

        if (result?.Message?.Content is null)
            throw new InvalidOperationException("O Ollama não retornou uma resposta válida.");

        var tokensPerSecond = CalculateTokensPerSecond(
            result.EvalCount,
            result.EvalDuration
        );

        return new LocalAiChatResponse(
            Provider: "Ollama Local",
            Model: result.Model ?? model,
            Response: result.Message.Content,
            TotalDurationNanoseconds: result.TotalDuration,
            LoadDurationNanoseconds: result.LoadDuration,
            PromptEvalCount: result.PromptEvalCount,
            PromptEvalDurationNanoseconds: result.PromptEvalDuration,
            EvalCount: result.EvalCount,
            EvalDurationNanoseconds: result.EvalDuration,
            TokensPerSecond: tokensPerSecond
        );
    }

    private static double CalculateTokensPerSecond(int evalCount, long evalDurationNanoseconds)
    {
        if (evalCount <= 0 || evalDurationNanoseconds <= 0)
            return 0;

        var seconds = evalDurationNanoseconds / 1_000_000_000.0;

        return evalCount / seconds;
    }

    private sealed record OllamaChatApiRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyCollection<OllamaMessage> Messages,
        [property: JsonPropertyName("stream")] bool Stream,
        [property: JsonPropertyName("options")] OllamaRequestOptions Options
    );

    private sealed record OllamaMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content
    );

    private sealed record OllamaRequestOptions(
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("num_predict")] int NumPredict
    );

    private sealed record OllamaChatApiResponse(
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("message")] OllamaMessage? Message,
        [property: JsonPropertyName("done")] bool Done,
        [property: JsonPropertyName("total_duration")] long TotalDuration,
        [property: JsonPropertyName("load_duration")] long LoadDuration,
        [property: JsonPropertyName("prompt_eval_count")] int PromptEvalCount,
        [property: JsonPropertyName("prompt_eval_duration")] long PromptEvalDuration,
        [property: JsonPropertyName("eval_count")] int EvalCount,
        [property: JsonPropertyName("eval_duration")] long EvalDuration
    );
}