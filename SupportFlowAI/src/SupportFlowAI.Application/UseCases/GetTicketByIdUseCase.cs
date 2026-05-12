using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;

namespace SupportFlowAI.Application.UseCases;

public sealed class GetTicketByIdUseCase
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketByIdUseCase(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id, cancellationToken);

        return ticket is null ? null : TicketResponse.FromEntity(ticket);
    }
}