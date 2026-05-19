using SupportFlowAI.Domain.Enums;
using SupportFlowAI.Domain.Exceptions;

namespace SupportFlowAI.Domain.ValueObjects;

public sealed record TicketAnalysis
{
    public TicketCategory Category { get; }
    public TicketPriority Priority { get; }
    public SentimentType Sentiment { get; }
    public double Confidence { get; }
    public string Summary { get; }
    public string SuggestedResponse { get; }
    public string ModelName { get; }

    public TicketAnalysis(
        TicketCategory category,
        TicketPriority priority,
        SentimentType sentiment,
        double confidence,
        string summary,
        string suggestedResponse,
        string modelName)
    {
        if (confidence is < 0 or > 1)
            throw new DomainException("A confiança da análise deve estar entre 0 e 1.");

        if (string.IsNullOrWhiteSpace(summary))
            throw new DomainException("O resumo da análise é obrigatório.");

        if (string.IsNullOrWhiteSpace(suggestedResponse))
            throw new DomainException("A resposta sugerida é obrigatória.");

        Category = category;
        Priority = priority;
        Sentiment = sentiment;
        Confidence = confidence;
        Summary = summary;
        SuggestedResponse = suggestedResponse;
        ModelName = modelName;
    }
}