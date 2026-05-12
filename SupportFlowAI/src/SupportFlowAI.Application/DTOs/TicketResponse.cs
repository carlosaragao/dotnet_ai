using SupportFlowAI.Domain.Entities;
using SupportFlowAI.Domain.Enums;

namespace SupportFlowAI.Application.DTOs;

public sealed record TicketResponse(
    Guid Id,
    string Title,
    string Description,
    string RequestedBy,
    TicketCategory Category,
    TicketPriority Priority,
    TicketStatus Status,
    DateTime CreatedAt
)
{
    public static TicketResponse FromEntity(Ticket ticket)
    {
        return new TicketResponse(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.RequestedBy,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.CreatedAt
        );
    }
}