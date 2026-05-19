using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Options;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Infrastructure.AzureAI;

public sealed class AzureLanguageTextAnalyzer : ICognitiveTextAnalyzer
{
    private readonly TextAnalyticsClient _client;

    public AzureLanguageTextAnalyzer(IOptions<AzureLanguageOptions> options)
    {
        var value = options.Value;

        if (string.IsNullOrWhiteSpace(value.Endpoint))
            throw new InvalidOperationException("Azure AI Language Endpoint não configurado.");

        if (string.IsNullOrWhiteSpace(value.Key))
            throw new InvalidOperationException("Azure AI Language Key não configurada.");

        _client = new TextAnalyticsClient(
            new Uri(value.Endpoint),
            new AzureKeyCredential(value.Key)
        );
    }

    public async Task<CognitiveTextAnalysisResult> AnalyzeAsync(
        string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Texto é obrigatório.");

        var sentimentResponse = await _client.AnalyzeSentimentAsync(
            text,
            cancellationToken: cancellationToken
        );

        var keyPhrasesResponse = await _client.ExtractKeyPhrasesAsync(
            text,
            cancellationToken: cancellationToken
        );

        var entitiesResponse = await _client.RecognizeEntitiesAsync(
            text,
            cancellationToken: cancellationToken
        );

        var sentiment = sentimentResponse.Value;
        var keyPhrases = keyPhrasesResponse.Value.ToArray();

        var entities = entitiesResponse.Value
            .Select(entity => $"{entity.Text} ({entity.Category})")
            .ToArray();

        return new CognitiveTextAnalysisResult(
            sentiment.Sentiment.ToString(),
            sentiment.ConfidenceScores.Positive,
            sentiment.ConfidenceScores.Neutral,
            sentiment.ConfidenceScores.Negative,
            keyPhrases,
            entities
        );
    }
}