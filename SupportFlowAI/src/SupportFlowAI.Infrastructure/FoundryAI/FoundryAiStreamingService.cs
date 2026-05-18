using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.FoundryAI;

public sealed class FoundryAiStreamingService : IFoundryAiStreamingService
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

    public FoundryAiStreamingService(
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

    public async IAsyncEnumerable<string> StreamAsync(
        string input,
        string? instructions,
        double? temperature,
        int? maxOutputTokens,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("A entrada é obrigatória.");

        if (temperature is < 0 or > 2)
            throw new ArgumentException("A temperature deve estar entre 0 e 2.");

        var finalMaxOutputTokens = maxOutputTokens ?? _options.DefaultMaxOutputTokens;

        if (finalMaxOutputTokens > _tokenBudgetOptions.MaxOutputTokens)
            throw new InvalidOperationException("O limite de tokens de saída excede o orçamento permitido.");

        var estimatedInputTokens = _tokenEstimator.EstimateTokens(
            $"{instructions}\n{input}"
        );

        if (estimatedInputTokens > _tokenBudgetOptions.MaxEstimatedInputTokens)
            throw new InvalidOperationException("A entrada excede o limite estimado de tokens permitido.");

        var requestBody = new FoundryStreamRequest(
            Model: _options.Model,
            Input: input,
            Instructions: instructions,
            Temperature: temperature ?? _options.DefaultTemperature,
            MaxOutputTokens: finalMaxOutputTokens,
            Stream: true
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, "responses")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody, JsonOptions),
                Encoding.UTF8,
                "application/json"
            )
        };

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Erro ao iniciar streaming no Foundry AI. Status: {(int)response.StatusCode}. Detalhes: {error}"
            );
        }

        var actualInputTokens = default(int?);
        var actualOutputTokens = default(int?);
        var actualTotalTokens = default(int?);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!line.StartsWith("data: ", StringComparison.OrdinalIgnoreCase))
                continue;

            var json = line["data: ".Length..].Trim();

            if (json == "[DONE]")
                break;

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var eventType = root.TryGetProperty("type", out var typeProperty)
                ? typeProperty.GetString()
                : null;

            if (eventType == "response.output_text.delta")
            {
                if (root.TryGetProperty("delta", out var deltaProperty))
                {
                    var delta = deltaProperty.GetString();

                    if (!string.IsNullOrEmpty(delta))
                        yield return delta;
                }
            }

            if (eventType == "response.completed")
            {
                TryReadUsage(
                    root,
                    out actualInputTokens,
                    out actualOutputTokens,
                    out actualTotalTokens
                );
            }

            if (eventType == "error" || eventType == "response.error")
            {
                var message = TryReadErrorMessage(root);

                throw new InvalidOperationException(
                    $"Erro retornado durante o streaming do Foundry AI: {message}"
                );
            }
        }

        await _usageRepository.AddAsync(
            new AiUsageRecord(
                Id: Guid.NewGuid(),
                Provider: "Microsoft Foundry AI",
                Operation: "responses-stream",
                Model: _options.Model,
                EstimatedInputTokens: estimatedInputTokens,
                ActualInputTokens: actualInputTokens,
                ActualOutputTokens: actualOutputTokens,
                ActualTotalTokens: actualTotalTokens,
                MaxOutputTokens: finalMaxOutputTokens,
                CreatedAt: DateTime.UtcNow
            ),
            cancellationToken
        );
    }

    private static void TryReadUsage(
        JsonElement root,
        out int? inputTokens,
        out int? outputTokens,
        out int? totalTokens)
    {
        inputTokens = null;
        outputTokens = null;
        totalTokens = null;

        if (!root.TryGetProperty("response", out var response))
            return;

        if (!response.TryGetProperty("usage", out var usage))
            return;

        if (usage.TryGetProperty("input_tokens", out var input))
            inputTokens = input.GetInt32();

        if (usage.TryGetProperty("output_tokens", out var output))
            outputTokens = output.GetInt32();

        if (usage.TryGetProperty("total_tokens", out var total))
            totalTokens = total.GetInt32();
    }

    private static string TryReadErrorMessage(JsonElement root)
    {
        if (root.TryGetProperty("error", out var error))
        {
            if (error.TryGetProperty("message", out var message))
                return message.GetString() ?? "Erro sem mensagem.";
        }

        if (root.TryGetProperty("message", out var rootMessage))
            return rootMessage.GetString() ?? "Erro sem mensagem.";

        return root.ToString();
    }

    private sealed record FoundryStreamRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("input")] string Input,
        [property: JsonPropertyName("instructions")] string? Instructions,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_output_tokens")] int MaxOutputTokens,
        [property: JsonPropertyName("stream")] bool Stream
    );
}