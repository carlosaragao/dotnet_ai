using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class ListTicketsUseCase
{
    private readonly ITicketRepository _ticketRepository;

    public ListTicketsUseCase(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<IReadOnlyCollection<TicketResponse>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.ListAsync(cancellationToken);

        return tickets
            .Select(TicketResponse.FromEntity)
            .ToList();
    }
}