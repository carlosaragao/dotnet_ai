namespace SupportFlowAI.Application.DTOs;

public sealed record CreateTicketRequest(
    string Title,
    string Description,
    string RequestedBy
);