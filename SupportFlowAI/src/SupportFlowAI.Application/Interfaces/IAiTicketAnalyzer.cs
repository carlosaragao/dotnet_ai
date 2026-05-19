using SupportFlowAI.Domain.Entities;
using SupportFlowAI.Domain.ValueObjects;

namespace SupportFlowAI.Application.Interfaces;

public interface IAiTicketAnalyzer
{
    Task<TicketAnalysis> AnalyzeAsync(Ticket ticket, CancellationToken cancellationToken);
}