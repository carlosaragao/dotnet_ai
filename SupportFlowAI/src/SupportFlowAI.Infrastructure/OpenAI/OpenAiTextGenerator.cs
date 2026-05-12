using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed class OpenAiTextGenerator : IAiTextGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;

    public OpenAiTextGenerator(
        HttpClient httpClient,
        IOptions<OpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AiTextGenerationResult> GenerateAsync(
        AiTextGenerationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            throw new ArgumentException("O prompt é obrigatório.");

        if (request.Temperature is < 0 or > 2)
            throw new ArgumentException("A temperature deve estar entre 0 e 2.");

        if (request.MaxOutputTokens <= 0)
            throw new ArgumentException("MaxOutputTokens deve ser maior que zero.");

        var model = string.IsNullOrWhiteSpace(request.Model)
            ? _options.ChatModel
            : request.Model;

        var openAiRequest = new OpenAiResponseRequest(
            Model: model,
            Instructions: request.SystemPrompt,
            Input: request.Prompt,
            Temperature: request.Temperature,
            MaxOutputTokens: request.MaxOutputTokens,
            TopP: request.TopP
        );

        var response = await _httpClient.PostAsJsonAsync(
            "responses",
            openAiRequest,
            JsonOptions,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao chamar OpenAI: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>(
            JsonOptions,
            cancellationToken
        );

        var outputText = ExtractOutputText(result);

        return new AiTextGenerationResult(
            Content: outputText,
            Model: result?.Model ?? _options.ChatModel,
            PromptTokens: result?.Usage?.InputTokens ?? 0,
            CompletionTokens: result?.Usage?.OutputTokens ?? 0,
            TotalTokens: result?.Usage?.TotalTokens ?? 0,
            FinishReason: result?.Status
        );
    }

    private static string ExtractOutputText(OpenAiResponse? response)
    {
        var text = response?
            .Output?
            .SelectMany(output => output.Content ?? [])
            .Where(content => content.Type == "output_text")
            .Select(content => content.Text)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("A OpenAI não retornou um texto válido.");

        return text;
    }

    private sealed record OpenAiResponseRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("instructions")] string? Instructions,
        [property: JsonPropertyName("input")] string Input,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_output_tokens")] int MaxOutputTokens,
        [property: JsonPropertyName("top_p")] double? TopP
    );

    private sealed record OpenAiResponse(
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("output")] List<OpenAiOutput>? Output,
        [property: JsonPropertyName("usage")] OpenAiUsage? Usage
    );

    private sealed record OpenAiOutput(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("content")] List<OpenAiOutputContent>? Content
    );

    private sealed record OpenAiOutputContent(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("text")] string? Text
    );

    private sealed record OpenAiUsage(
        [property: JsonPropertyName("input_tokens")] int InputTokens,
        [property: JsonPropertyName("output_tokens")] int OutputTokens,
        [property: JsonPropertyName("total_tokens")] int TotalTokens
    );
}