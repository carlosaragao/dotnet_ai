using SupportFlowAI.Domain.Entities;
using SupportFlowAI.Domain.Enums;

namespace SupportFlowAI.Application.DTOs;

public sealed record AnalyzeTicketResponse(
    Guid TicketId,
    TicketCategory Category,
    TicketPriority Priority,
    SentimentType Sentiment,
    double Confidence,
    string Summary,
    string SuggestedResponse,
    string ModelName
)
{
    public static AnalyzeTicketResponse FromEntity(Ticket ticket)
    {
        if (ticket.Analysis is null)
            throw new InvalidOperationException("O chamado ainda não possui análise.");

        return new AnalyzeTicketResponse(
            ticket.Id,
            ticket.Analysis.Category,
            ticket.Analysis.Priority,
            ticket.Analysis.Sentiment,
            ticket.Analysis.Confidence,
            ticket.Analysis.Summary,
            ticket.Analysis.SuggestedResponse,
            ticket.Analysis.ModelName
        );
    }
}