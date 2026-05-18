namespace SupportFlowAI.Application.DTOs.Media;

public sealed record CreateTicketFromMediaResponse(
    Guid TicketId,
    string Title,
    string ExtractedContent,
    string SourceType
);