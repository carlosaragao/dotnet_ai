using SupportFlowAI.Application.DTOs;
using SupportFlowAI.Application.Interfaces;
using SupportFlowAI.Domain.Entities;

namespace SupportFlowAI.Application.UseCases;

public sealed class CreateTicketUseCase
{
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketUseCase(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketResponse> HandleAsync(
        CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var ticket = new Ticket(
            request.Title,
            request.Description,
            request.RequestedBy
        );

        await _ticketRepository.AddAsync(ticket, cancellationToken);

        return TicketResponse.FromEntity(ticket);
    }
}