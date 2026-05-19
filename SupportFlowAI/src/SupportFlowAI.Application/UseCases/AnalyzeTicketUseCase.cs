using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class AnalyzeTicketUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAiTicketAnalyzer _aiTicketAnalyzer;

    public AnalyzeTicketUseCase(
        ITicketRepository ticketRepository,
        IAiTicketAnalyzer aiTicketAnalyzer)
    {
        _ticketRepository = ticketRepository;
        _aiTicketAnalyzer = aiTicketAnalyzer;
    }

    public async Task<AnalyzeTicketResponse> HandleAsync(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            throw new KeyNotFoundException("Chamado não encontrado.");

        var analysis = await _aiTicketAnalyzer.AnalyzeAsync(ticket, cancellationToken);

        ticket.ApplyAnalysis(analysis);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        return AnalyzeTicketResponse.FromEntity(ticket);
    }
}