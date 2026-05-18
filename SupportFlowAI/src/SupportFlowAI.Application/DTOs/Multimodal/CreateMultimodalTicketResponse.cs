namespace SupportFlowAI.Application.DTOs.Multimodal;

public sealed record CreateMultimodalTicketResponse(
    Guid TicketId,
    string Title,
    string Description,
    string Analysis
);