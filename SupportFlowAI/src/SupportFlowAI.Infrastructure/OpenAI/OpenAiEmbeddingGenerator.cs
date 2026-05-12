using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed class OpenAiEmbeddingGenerator : IEmbeddingGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;

    public OpenAiEmbeddingGenerator(
        HttpClient httpClient,
        IOptions<OpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AiEmbeddingResult> GenerateAsync(
        string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("O texto para embedding é obrigatório.");

        var request = new OpenAiEmbeddingRequest(
            Model: _options.EmbeddingModel,
            Input: text,
            EncodingFormat: "float"
        );

        var response = await _httpClient.PostAsJsonAsync(
            "embeddings",
            request,
            JsonOptions,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao gerar embedding na OpenAI: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAiEmbeddingResponse>(
            JsonOptions,
            cancellationToken
        );

        var embedding = result?.Data?.FirstOrDefault()?.Embedding;

        if (embedding is null || embedding.Length == 0)
            throw new InvalidOperationException("A OpenAI não retornou um vetor de embedding válido.");

        return new AiEmbeddingResult(
            new EmbeddingVector(embedding),
            result?.Model ?? _options.EmbeddingModel,
            result?.Usage?.PromptTokens ?? 0,
            result?.Usage?.TotalTokens ?? 0
        );
    }

    private sealed record OpenAiEmbeddingRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("input")] string Input,
        [property: JsonPropertyName("encoding_format")] string EncodingFormat
    );

    private sealed record OpenAiEmbeddingResponse(
        [property: JsonPropertyName("data")] List<OpenAiEmbeddingData> Data,
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("usage")] OpenAiEmbeddingUsage? Usage
    );

    private sealed record OpenAiEmbeddingData(
        [property: JsonPropertyName("embedding")] float[] Embedding,
        [property: JsonPropertyName("index")] int Index
    );

    private sealed record OpenAiEmbeddingUsage(
        [property: JsonPropertyName("prompt_tokens")] int PromptTokens,
        [property: JsonPropertyName("total_tokens")] int TotalTokens
    );
}