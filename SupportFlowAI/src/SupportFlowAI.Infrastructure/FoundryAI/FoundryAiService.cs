using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI;
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
    private readonly TokenBudgetOptions _tokenBudgetOptions;
    private readonly ITextTokenEstimator _tokenEstimator;
    private readonly IAiUsageRepository _usageRepository;

    public FoundryAiService(
        HttpClient httpClient,
        IOptions<FoundryAiOptions> options,
        IOptions<TokenBudgetOptions> tokenBudgetOptions,
        ITextTokenEstimator tokenEstimator,
        IAiUsageRepository usageRepository)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _tokenBudgetOptions = tokenBudgetOptions.Value;
        _tokenEstimator = tokenEstimator;
        _usageRepository = usageRepository;
    }

    public async Task<FoundryAiCompletionResponse> GenerateAsync(
        FoundryAiCompletionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Input))
            throw new ArgumentException("A entrada é obrigatória.");

        if (request.Temperature is < 0 or > 2)
            throw new ArgumentException("A temperature deve estar entre 0 e 2.");

        var maxOutputTokens = request.MaxOutputTokens ?? _options.DefaultMaxOutputTokens;

        if (maxOutputTokens > _tokenBudgetOptions.MaxOutputTokens)
            throw new InvalidOperationException("O limite de tokens de saída excede o orçamento permitido.");

        var estimatedInputTokens = _tokenEstimator.EstimateTokens(
            $"{request.Instructions}\n{request.Input}"
        );

        if (estimatedInputTokens > _tokenBudgetOptions.MaxEstimatedInputTokens)
            throw new InvalidOperationException("A entrada excede o limite estimado de tokens permitido.");

        var foundryRequest = new FoundryResponsesRequest(
            Model: _options.Model,
            Input: request.Input,
            Instructions: request.Instructions,
            Temperature: request.Temperature ?? _options.DefaultTemperature,
            MaxOutputTokens: maxOutputTokens,
            Stream: null
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

        await _usageRepository.AddAsync(
            new AiUsageRecord(
                Id: Guid.NewGuid(),
                Provider: "Microsoft Foundry AI",
                Operation: "responses",
                Model: result?.Model ?? _options.Model,
                EstimatedInputTokens: estimatedInputTokens,
                ActualInputTokens: result?.Usage?.InputTokens,
                ActualOutputTokens: result?.Usage?.OutputTokens,
                ActualTotalTokens: result?.Usage?.TotalTokens,
                MaxOutputTokens: maxOutputTokens,
                CreatedAt: DateTime.UtcNow
            ),
            cancellationToken
        );

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
        [property: JsonPropertyName("max_output_tokens")] int MaxOutputTokens,
        [property: JsonPropertyName("stream")] bool? Stream
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