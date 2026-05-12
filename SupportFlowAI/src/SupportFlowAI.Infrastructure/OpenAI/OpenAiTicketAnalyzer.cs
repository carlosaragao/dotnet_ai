using System.Text.Json;
using SupportFlowAI.Application.AI;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;
using SupportFlowAI.Domain.Enums;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Infrastructure.OpenAI;

public sealed class OpenAiTicketAnalyzer : IAiTicketAnalyzer
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAiTextGenerator _textGenerator;

    public OpenAiTicketAnalyzer(IAiTextGenerator textGenerator)
    {
        _textGenerator = textGenerator;
    }

    public async Task<TicketAnalysis> AnalyzeAsync(
        Ticket ticket,
        CancellationToken cancellationToken)
    {
        var systemPrompt = """
        Você é um classificador de chamados corporativos.

        Sua tarefa é analisar o chamado recebido e retornar exclusivamente um JSON válido,
        sem markdown, sem explicações e sem texto adicional.

        Categorias possíveis:
        - Access
        - Finance
        - Infrastructure
        - TechnicalSupport
        - HumanResources

        Prioridades possíveis:
        - Low
        - Medium
        - High
        - Critical

        Sentimentos possíveis:
        - Neutral
        - Positive
        - Negative

        O JSON deve seguir exatamente este formato:
        {
          "category": "Access",
          "priority": "High",
          "sentiment": "Negative",
          "confidence": 0.91,
          "summary": "Resumo curto do problema.",
          "suggestedResponse": "Resposta inicial sugerida ao usuário."
        }
        """;

        var prompt = $"""
        Analise o chamado abaixo.

        Título:
        {ticket.Title}

        Descrição:
        {ticket.Description}

        Solicitante:
        {ticket.RequestedBy}
        """;

        var result = await _textGenerator.GenerateAsync(
            new AiTextGenerationRequest(
                Prompt: prompt,
                SystemPrompt: systemPrompt,
                Temperature: 0.1,
                MaxOutputTokens: 500
            ),
            cancellationToken
        );

        var dto = JsonSerializer.Deserialize<OpenAiTicketAnalysisDto>(
            CleanJson(result.Content),
            JsonOptions
        );

        if (dto is null)
            throw new InvalidOperationException("Não foi possível interpretar a análise retornada pela OpenAI.");

        return new TicketAnalysis(
            ParseCategory(dto.Category),
            ParsePriority(dto.Priority),
            ParseSentiment(dto.Sentiment),
            dto.Confidence,
            dto.Summary,
            dto.SuggestedResponse,
            result.Model
        );
    }

    private static string CleanJson(string content)
    {
        return content
            .Replace("```json", string.Empty)
            .Replace("```", string.Empty)
            .Trim();
    }

    private static TicketCategory ParseCategory(string value)
    {
        return Enum.TryParse<TicketCategory>(value, ignoreCase: true, out var result)
            ? result
            : TicketCategory.Unknown;
    }

    private static TicketPriority ParsePriority(string value)
    {
        return Enum.TryParse<TicketPriority>(value, ignoreCase: true, out var result)
            ? result
            : TicketPriority.Medium;
    }

    private static SentimentType ParseSentiment(string value)
    {
        return Enum.TryParse<SentimentType>(value, ignoreCase: true, out var result)
            ? result
            : SentimentType.Neutral;
    }
}