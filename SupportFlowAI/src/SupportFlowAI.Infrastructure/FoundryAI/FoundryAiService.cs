using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.DTOs.FoundryAI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.FoundryAI;

public sealed class FoundryAiService : IFoundryAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly FoundryAiOptions _options;

    public FoundryAiService(
        HttpClient httpClient,
        IOptions<FoundryAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<FoundryAiCompletionResponse> GenerateAsync(
        FoundryAiCompletionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Input))
            throw new ArgumentException("A entrada é obrigatória.");

        if (request.Temperature is < 0 or > 2)
            throw new ArgumentException("A temperature deve estar entre 0 e 2.");

        var foundryRequest = new FoundryResponsesRequest(
            Model: _options.Model,
            Input: request.Input,
            Instructions: request.Instructions,
            Temperature: request.Temperature ?? _options.DefaultTemperature,
            MaxOutputTokens: request.MaxOutputTokens ?? _options.DefaultMaxOutputTokens
        );

        var response = await _httpClient.PostAsJsonAsync(
            "responses",
            foundryRequest,
            JsonOptions,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Erro ao chamar Microsoft Foundry AI. Status: {(int)response.StatusCode}. Detalhes: {error}"
            );
        }

        var result = await response.Content.ReadFromJsonAsync<FoundryResponsesResponse>(
            JsonOptions,
            cancellationToken
        );

        var outputText = ExtractOutputText(result);

        return new FoundryAiCompletionResponse(
            Provider: "Microsoft Foundry AI",
            Model: result?.Model ?? _options.Model,
            Response: outputText,
            InputTokens: result?.Usage?.InputTokens ?? 0,
            OutputTokens: result?.Usage?.OutputTokens ?? 0,
            TotalTokens: result?.Usage?.TotalTokens ?? 0,
            Status: result?.Status
        );
    }

    private static string ExtractOutputText(FoundryResponsesResponse? response)
    {
        var text = response?
            .Output?
            .SelectMany(output => output.Content ?? [])
            .Where(content => content.Type == "output_text")
            .Select(content => content.Text)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("O Foundry AI não retornou um texto válido.");

        return text;
    }

    private sealed record FoundryResponsesRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("input")] string Input,
        [property: JsonPropertyName("instructions")] string? Instructions,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_output_tokens")] int MaxOutputTokens
    );

    private sealed record FoundryResponsesResponse(
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("output")] List<FoundryOutput>? Output,
        [property: JsonPropertyName("usage")] FoundryUsage? Usage
    );

    private sealed record FoundryOutput(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("content")] List<FoundryOutputContent>? Content
    );

    private sealed record FoundryOutputContent(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("text")] string? Text
    );

    private sealed record FoundryUsage(
        [property: JsonPropertyName("input_tokens")] int InputTokens,
        [property: JsonPropertyName("output_tokens")] int OutputTokens,
        [property: JsonPropertyName("total_tokens")] int TotalTokens
    );
}